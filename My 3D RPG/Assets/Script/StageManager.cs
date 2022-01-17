using ProjectChan.Battle;
using ProjectChan.DB;
using ProjectChan.Define;
using ProjectChan.Object;
using ProjectChan.Resource;
using ProjectChan.UI;
using ProjectChan.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace ProjectChan
{
    using Monster = ProjectChan.Object.Monster;

    /// <summary>
    /// => 스테이지 관련 기능들을 수행할 클래스
    /// => 스테이지 전환 시 처리 작업을 수행함(해당 스테이지에 필요한 리소스 로드 및 인스턴스 생성)
    /// </summary>
    public class StageManager : Singleton<StageManager>
    {
        public int huntedMon;                       // -> 사냥한 몬스터 개수
        public bool isMonReady;                     // -> 몬스터를 생성할 준비가되었는가?
        public Transform monsterHolder;             // -> 몬스터를 자식으로 가질 부모 홀더

        private GameObject currentStage;            // -> 현재 스테이지 객체
        private Transform NPCHolder;                // -> NPC를 자식으로 가질 부모 홀더
        private float currentMonSpawnTime;          // -> 현재 몬스터 스폰 시간
        private float maxMonSpawnTime;              // -> 최대 몬스터 스폰 시간

        /// <summary>
        /// => 몬스터를 생성할 스폰 지역
        /// </summary>
        private Dictionary<int, Bounds> spawnAreaBounds = new Dictionary<int, Bounds>();

        private void Update()
        {
            // -> 몬스터를 생성할 준비가 되어있지 않다면!
            if (!isMonReady)
            {
                return;
            }

            // -> 몬스터 스폰 타입을 계속 업데이트 해줍니다!
            CheckSpawnTime();
        }

        /// <summary>
        /// => 프리팹으로 저장된 스테이지를 불러오는 메서드
        /// </summary>
        /// <returns></returns>
        public IEnumerator ChangeStage()
        {
            // -> 아직 몬스터 생성 준비가 되어있지 않습니다!
            isMonReady = false;

            var sdStage = GameManager.User.boStage.sdStage;
            var resourceManager = ResourceManager.Instance;
            var objectPoolManager = ObjectPoolManager.Instance;

            // -> 현재 스테이지가 존재한다면!
            if (currentStage != null)
            {
                Destroy(currentStage);
            }

            // -> ResourceManager의 LoadObject와 스테이지 기획 데이터를 이용하여 현재 스테이지를 생성합니다!
            currentStage = Instantiate(resourceManager.LoadObject(sdStage.resourcePath));

            // -> 현재 스테이지의 오디오 클립을 설정합니다!
            switch (sdStage.resourcePath.Remove(0, sdStage.resourcePath.LastIndexOf('/') + 1))
            {
                case "StartVillage":
                    AudioManager.Instance.ChangeAudioClip(Audio.ClipType.Village);
                    break;

                case "DunGeon":
                    AudioManager.Instance.ChangeAudioClip(Audio.ClipType.DunGeon);
                    break;
            }

            // -> 로딩 씬을 보여주는 과정에서 로딩 씬에 오브젝트들이 생성되기 때문에 InGame으로 오브젝트를 옮겨줍니다!
            SceneManager.MoveGameObjectToScene(currentStage, SceneManager.GetSceneByName(SceneType.InGame.ToString()));

            spawnAreaBounds.Clear();

            // -> 몬스터 및 NPC를 삭제
            objectPoolManager.ClearPool<Monster>(PoolType.Monster);

            var battleManager = BattleManager.Instance;
            battleManager.Monsters.Clear();
            battleManager.ClearNPC();

            var sdMonsters = GameManager.SD.sdMonsters;

            // -> 몬스터를 Pool에 넣어놓는 작업
            for (int i = 0; i < sdStage.genMonsters.Length; i++)
            {
                var sdMonster = sdMonsters.Where(obj => obj.index == sdStage.genMonsters[i]).SingleOrDefault();

                // -> 현재 스테이지에 생성할 몬스터가 존재한다면!
                if (sdMonster != null)
                {
                    // -> 풀에 몬스터를 만들어줍니다!
                    resourceManager.LoadPoolableObject<Monster>(PoolType.Monster, sdMonster.resourcePath, 5);
                }
                else
                {
                    continue;
                }

                // -> 스테이지 기획 데이터에서 스폰할 지역의 인덱스 값을 가져옵니다!
                var spawnAreaIndex = sdStage.spawnArea[i];

                // -> 스폰 지역이 존재한다면!
                if (spawnAreaIndex != -1)
                {
                    if (!spawnAreaBounds.ContainsKey(spawnAreaIndex))
                    {
                        // -> 현재 스테이지에 존재하는 SpawnPosHolder에서 얻은 인덱스 값을 이용해서 스폰 지역을 가져옵니다!
                        var spawnArea = currentStage.transform.Find("SpawnPosHolder").GetChild(spawnAreaIndex);

                        // -> 가져온 스폰 지역을 딕셔너리에 담아놓습니다!
                        spawnAreaBounds.Add(spawnAreaIndex, spawnArea.GetComponent<Collider>().bounds);
                    }
                }
            }

            yield return null;
        }

        /// <summary>
        /// => 스테이지 불러오는 것을 완료하고 나서 실행할 메서드
        /// </summary>
        public void OnChangeStageComplete()
        {
            // -> 현재 스테이지에 존재하는 몬스터 체력바를 풀로 반환 합니다!
            UIWindowManager.Instance.GetWindow<UIBattle>().Clear();

            ClearSpawnTime();
            SpawnNPC();
            SpawnCharacter();
            SpawnMonster();

            // -> 몬스터를 생성할 준비가 되었습니다!
            isMonReady = true;
        }

        #region 오브젝트 생성 작업

        /// <summary>
        /// => 캐릭터 생성 또는 스테이지 이동 시 캐릭터 위치를 재설정 해주는 메서드
        /// </summary>
        private void SpawnCharacter()
        {
            var playerController = FindObjectOfType<PlayerController>();

            if (playerController == null)
            {
                return;
            }

            // -> 플레이어 컨트롤러가 있다면!
            if (playerController.PlayerCharacter != null)
            {
                // -> 씬 이동이 아니라 스테이지를 이동 했다는 의미입니다!
                var warpEntry = currentStage.transform.Find
                    ($"WarpPosHolder/{GameManager.User.boStage.prevStageIndex}/EntryPos").transform;

                // -> 캐릭터의 Position을 설정합니다
                playerController.PlayerCharacter.transform.position = warpEntry.position;
                playerController.PlayerCharacter.transform.forward = warpEntry.forward;

                // -> 캐릭터 이동에 따라 카메라도 이동시켜줍니다!
                playerController.cameraController.SetForceStandarView();
                return;
            }

            // -> 씬 이동을 했다면 캐릭터가 아직 생성 되어있지 않았으므로 생성 해줍니다!
            var characterObj = Instantiate(ResourceManager.Instance.LoadObject
                (GameManager.User.boCharacter.sdCharacter.resourcePath));
            characterObj.transform.position = GameManager.User.boStage.prevPos;

            // -> 생성한 캐릭터를 초기화 해줍니다!
            var playerCharacter = characterObj.GetComponent<Character>();
            playerCharacter.Initialize(GameManager.User.boCharacter);

            // -> 컨트롤러에 캐릭터를 저장합니다!
            playerController.Initialize(playerCharacter);

            // -> 배틀 매니저에 등록합니다!
            BattleManager.Instance.AddActor(playerCharacter);
        }

        /// <summary>
        /// => NPC를 생성하는 메서드
        /// </summary>
        private void SpawnNPC()
        {
            // -> NPC홀더가 존재하지 않는다면!
            if (NPCHolder == null)
            {
                // -> 홀더를 생성합니다!
                NPCHolder = new GameObject("NPCHolder").transform;
                NPCHolder.position = Vector3.zero;
            }

            var stageIndex = GameManager.User.boStage.sdStage.index;
            var npcs = GameManager.SD.sdNPCs.Where(obj => obj.stageIndex == stageIndex)?.ToList();
            var battleManager = BattleManager.Instance;

            for (int i = 0; i < npcs.Count; i++)
            {
                var npcObj = Instantiate(ResourceManager.Instance.LoadObject(npcs[i].resourcePath), NPCHolder);
                var npc = npcObj?.GetComponent<NPC>();
                var boNPC = new BoNPC(npcs[i]);

                // -> 생성한 NPC를 세팅합니다!
                npc.Initialize(boNPC);

                // -> 생성한 NPC를 배틀 매니저에 등록합니다!
                battleManager.AddNPC(npc);
            }
        }

        /// <summary>
        /// => 몬스터의 스폰시간을 체크하여 몬스터를 생성할지 안할지 판단하는 메서드
        /// </summary>
        private void CheckSpawnTime()
        {
            if (currentStage == null)
            {
                return;
            }

            currentMonSpawnTime += Time.deltaTime;

            if (currentMonSpawnTime >= maxMonSpawnTime)
            {
                ClearSpawnTime();
                SpawnMonster();
            }
        }

        /// <summary>
        /// => 몬스터의 스폰시간을 초기화 하는 메서드
        /// </summary>
        private void ClearSpawnTime()
        {
            currentMonSpawnTime = 0;
            maxMonSpawnTime = Random.Range(Spawn.MinMonsterSpawnTime, Spawn.MaxMonsterSpawnTime);
        }

        /// <summary>
        /// => 몬스터를 불러오는 작업을 하는 메서드
        /// </summary>
        private void SpawnMonster()
        {
            // -> 홀더가 없다면!
            if (monsterHolder == null)
            {
                // -> 없다면 생성해줍니다!
                monsterHolder = new GameObject("MonsterHolder").transform;
                monsterHolder.position = Vector3.zero;
            }

            var sd = GameManager.SD;
            var sdStage = GameManager.User.boStage.sdStage;

            // -> 스테이지에 생성된 몬스터의 개수와 생성 제한 값이 같다면!
            if (monsterHolder.childCount >= sdStage.stageMonCount)
            {
                // -> 생성 작업을 멈춥니다!
                return;
            }

            // -> 생성할 몬스터 개수를 정해줍니다!
            var monsterSpawnCnt = Random.Range(Spawn.MinMonsterSpawnCnt, Spawn.MaxMonsterSpawnCnt);
            var monsterPool = ObjectPoolManager.Instance.GetPool<Monster>(PoolType.Monster);
            var battleManager = BattleManager.Instance;

            for (int i = 0; i < monsterSpawnCnt; i++)
            {
                // -> 현재 스테이지에서 생성되는 몬스터 배열의 길이를 이용해서 랜덤 값을 하나 생성 합니다!
                var randIndex = Random.Range(0, sdStage.genMonsters.Length);

                // -> 랜덤 값을 이용하여 생성할 몬스터 인덱스를 가져옵니다!
                var genMonsterIndex = sdStage.genMonsters[randIndex];

                // -> 생성할 몬스터가 없다면!
                if (genMonsterIndex == -1)
                {
                    return;
                }

                // -> 가져온 인덱스가 보스 인덱스라면!
                if (genMonsterIndex == StaticData.BossIndex)
                {
                    // -> 보스 생성 조건을 만족했다면!
                    if (huntedMon >= StaticData.BossSpawn)
                    {
                        huntedMon = 0;
                    }
                    else
                    {
                        // -> 위로 올라갑니다!
                        continue;
                    }
                }

                // -> 가져온 몬스터 인덱스를 이용하여 기획 데이터를 가져옵니다!
                var sdMonster = sd.sdMonsters.Where(obj => obj.index == genMonsterIndex).SingleOrDefault();
                var monster = monsterPool.GetPoolableObject(obj => obj.name == sdMonster.name);

                // -> 생성되는 몬스터가 없다면 다시 위로 갑니다!
                if (monster == null)
                {
                    continue;
                }

                // -> 어디에 스폰될지를 정해줍니다!
                var bounds = spawnAreaBounds[sdStage.spawnArea[randIndex]];
                var spawnPosX = Random.Range(-bounds.size.x * 0.5f, bounds.size.x * 0.5f);
                var spawnPosZ = Random.Range(-bounds.size.z * 0.5f, bounds.size.z * 0.5f);
                var centerPos = new Vector3(bounds.center.x, 0, bounds.center.z);

                // -> 몬스터가 생성될 위치에 레이를 쏴 그 지점의 Y값을 얻으려는 작업입니다!
                transform.position = centerPos + new Vector3(spawnPosX, 50f, spawnPosZ);
                Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 55f, 1 << LayerMask.NameToLayer("Floor"));
                Debug.DrawRay(transform.position, Vector3.down * 55f, Color.red);

                // -> 만약 레이에 닿은 터레인이 있다면 몬스터를 생성합니다!
                monster.transform.position = centerPos + new Vector3(spawnPosX, hit.point.y, spawnPosZ);
                monster.transform.SetParent(monsterHolder, true);
                monster.Initialize(new BoMonster(sdMonster));
                monster.State = Define.Actor.ActorState.None;
                battleManager.AddActor(monster);
            }
        }

        /// <summary>
        /// => 에이전트의 목적지(destPos)를 반환해주는 메서드
        /// </summary>
        /// <param name="monsterIndex"> 생성될 몬스터 인덱스 값 </param>
        /// <returns></returns>
        public Vector3 GetRandPosInArea(int monsterIndex)
        {
            var sdStage = GameManager.User.boStage.sdStage;

            var arrayIndex = -1;

            // -> 목적지를 받을 몬스터의 인덱스가 현재 스테이지에 포함되어 있는지 확인하는 작업입니다!
            for (int i = 0; i < sdStage.genMonsters.Length; i++)
            {
                // -> 같은 값이 존재한다면!
                if (sdStage.genMonsters[i] == monsterIndex)
                {
                    arrayIndex = i;
                    break;
                }
            }

            // -> 딕셔너리에 저장되어 있는 Bounds 정보를 가져옵니다!
            var bounds = spawnAreaBounds[sdStage.spawnArea[arrayIndex]];
            var spawnPosX = Random.Range(-bounds.size.x * .5f, bounds.size.x * .5f);
            var spawnPosZ = Random.Range(-bounds.size.z * .5f, bounds.size.z * .5f);

            transform.position = new Vector3(spawnPosX, 50f, spawnPosZ);
            Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 55f, 1 << LayerMask.NameToLayer("Floor"));
            Debug.DrawRay(transform.position, Vector3.down * 55f, Color.red);

            // -> 목적지를 반환해줍니다!
            return new Vector3(spawnPosX, hit.point.y, spawnPosZ);
        }

        #endregion
    }
}