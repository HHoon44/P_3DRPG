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

    public class StageManager : Singleton<StageManager>
    {
        private GameObject currentStage;            // -> 현재 스테이지 객체
        private Transform NPCHolder;                // -> NPC를 자식으로 가질 부모 홀더
        private float currentMonSpawnTime;          // -> 현재 몬스터 스폰 시간
        private float maxMonSpawnTime;              // -> 최대 몬스터 스폰 시간

        public int huntedMon = 10;                       // -> 사냥한 몬스터 개수
        public bool isMonReady;                     // -> 몬스터를 생성할 준비가되었는가?
        public Transform monsterHolder;             // -> 몬스터를 자식으로 가질 부모 홀더

        /// <summary>
        /// => 몬스터를 생성할 스폰 지역
        /// </summary>
        private Dictionary<int, Bounds> spawnAreaBounds = new Dictionary<int, Bounds>();

        private void Update()
        {
            if (!isMonReady)
            {
                return;
            }

            CheckSpawnTime();
        }

        /// <summary>
        /// => 프리팹으로 저장된 스테이지를 불러오는 메서드
        /// </summary>
        /// <returns></returns>
        public IEnumerator ChangeStage()
        {
            isMonReady = false;

            var sdStage = GameManager.User.boStage.sdStage;
            var resourceManager = ResourceManager.Instance;
            var objectPoolManager = ObjectPoolManager.Instance;

            if (currentStage != null)
            {
                Destroy(currentStage);
            }

            currentStage = Instantiate(resourceManager.LoadObject(sdStage.resourcePath));

            // -> 로딩씬을 불러오는 과정에서 로딩씬에 object들이 생성되기 때문에 InGame으로 옮겨준다
            SceneManager.MoveGameObjectToScene(currentStage, SceneManager.GetSceneByName(SceneType.InGame.ToString()));

            spawnAreaBounds.Clear();
            objectPoolManager.ClearPool<Monster>(PoolType.Monster);

            // -> 몬스터 및 NPC를 삭제
            var battleManager = BattleManager.Instance;
            battleManager.Monsters.Clear();
            battleManager.ClearNPC();

            var sd = GameManager.SD;

            // -> 몬스터를 Pool에 넣어놓는 작업
            for (int i = 0; i < sdStage.genMonsters.Length; i++)
            {
                var sdMonster = sd.sdMonsters.Where(obj => obj.index == sdStage.genMonsters[i]).SingleOrDefault();

                if (sdMonster != null)
                {
                    resourceManager.LoadPoolableObject<Monster>(PoolType.Monster, sdMonster.resourcePath, 5);
                }
                else
                {
                    continue;
                }

                // -> 스테이지 기획 데이터에 저장된 spawnArea 배열의 i인덱스에 접근하여 값을 가져온다
                var spawnAreaIndex = sdStage.spawnArea[i];

                if (spawnAreaIndex != -1)
                {
                    if (!spawnAreaBounds.ContainsKey(spawnAreaIndex))
                    {
                        // -> 현재 스테이지에서 SpawnPosHolder를 찾아 spawnAreaIndex번째 자식 객체를 가져온다
                        // -> 가져왔으면 Bounds 리스트에 추가한다
                        var spawnArea = currentStage.transform.Find("SpawnPosHolder").GetChild(spawnAreaIndex);
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
            // -> 현재 스테이지에 존재하는 몬스터 체력바를 다시 풀로 반환
            UIWindowManager.Instance.GetWindow<UIBattle>().Clear();

            ClearSpawnTime();
            SpawnNPC();
            SpawnCharacter();
            SpawnMonster();

            isMonReady = true;
        }

        #region 오브젝트 생성 작업

        /// <summary>
        /// => 캐릭터 생성 또는 스테이지 이동시 캐릭터 위치를 재설정 해주는 메서드
        /// </summary>
        private void SpawnCharacter()
        {
            var playerController = FindObjectOfType<PlayerController>();

            if (playerController == null)
            {
                return;
            }

            // -> 캐릭터가 씬 이동이 아니라 스테이지 이동을 했다는 의미
            if (playerController.PlayerCharacter != null)
            {
                // -> 워프한 캐릭터가 위치할 EntryPos찾기
                var warpEntry = currentStage.transform.Find
                    ($"WarpPosHolder/{GameManager.User.boStage.prevStageIndex}/EntryPos").transform;

                // -> 캐릭터 Pos 설정
                playerController.PlayerCharacter.transform.position = warpEntry.position;
                playerController.PlayerCharacter.transform.forward = warpEntry.forward;

                // -> 캐릭터 이동에 따라 카메라도 이동시켜줌
                playerController.cameraController.SetForceStandarView();
                return;
            }

            var characterObj = Instantiate(ResourceManager.Instance.LoadObject
                (GameManager.User.boCharacter.sdCharacter.resourcePath));
            characterObj.transform.position = GameManager.User.boStage.prevPos;

            var playerCharacter = characterObj.GetComponent<Character>();
            playerCharacter.Initialize(GameManager.User.boCharacter);

            playerController.Initialize(playerCharacter);

            BattleManager.Instance.AddActor(playerCharacter);
        }

        /// <summary>
        /// => NPC를 생성하는 메서드
        /// </summary>
        private void SpawnNPC()
        {
            // -> 홀더가 없다면 생성하고 Pos를 초기화
            if (NPCHolder == null)
            {
                NPCHolder = new GameObject("NPCHolder").transform;
                NPCHolder.position = Vector3.zero;
            }

            // -> NPC 생성할때 특정 퀘스트에서만 생성되는 NPC가 있다면 예외처리 해야함 아직 안함 

            var stageIndex = GameManager.User.boStage.sdStage.index;
            var npcs = GameManager.SD.sdNPCs.Where(obj => obj.stageIndex == stageIndex)?.ToList();
            var battleManager = BattleManager.Instance;

            for (int i = 0; i < npcs.Count; i++)
            {
                var npcObj = Instantiate(ResourceManager.Instance.LoadObject(npcs[i].resourcePath), NPCHolder);
                var npc = npcObj?.GetComponent<NPC>();
                var boNPC = new BoNPC(npcs[i]);
                npc.Initialize(boNPC);
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
            if (monsterHolder == null)
            {
                monsterHolder = new GameObject("MonsterHolder").transform;
                monsterHolder.position = Vector3.zero;
            }

            // -> 현재 스테이지 정보를 가져온다
            var sd = GameManager.SD;
            var sdStage = GameManager.User.boStage.sdStage;

            // -> 생성할 몬스터 갯수
            var monsterSpawnCnt = Random.Range(Spawn.MinMonsterSpawnCnt, Spawn.MaxMonsterSpawnCnt);
            var monsterPool = ObjectPoolManager.Instance.GetPool<Monster>(PoolType.Monster);
            var battleManager = BattleManager.Instance;

            for (int i = 0; i < monsterSpawnCnt; i++)
            {
                // -> 현재 스테이지에서 생성되는 몬스터 배열의 길이를 이용하여 랜덤 값을 하나 생성
                var randIndex = Random.Range(0, sdStage.genMonsters.Length);

                // -> 배열에 랜덤 값을 이용하여 배열안에 값을 가져온다
                var genMonsterIndex = sdStage.genMonsters[randIndex];

                // -> -1이라면 생성되는 몬스터가 없으므로 return
                if (genMonsterIndex == -1)
                {
                    return;
                }

                /// 설마 보스 몬스터 인덱스 가져온거임?
                if (genMonsterIndex == StaticData.BossIndex)
                {
                    /// 근데 보스 몬스터 소환 할려면 몬스터 10마리 이상은 잡아야하는데 괜춘?
                    if (huntedMon >= 10)
                    {
                        /// 뭐야 잡아놨네 그러면 보스 몬스터 소환 ㄱㄱ
                        huntedMon = 0;
                    }
                    else
                    {
                        /// 뭐야 아직 못잡았네 다시 위로 올라가셈~
                        continue;
                    }
                }

                // -> 기획 데이터에 위에서 얻은 몬스터 인덱스와 같은 인덱스 값을 가진 데이터를 가져온다
                var sdMonster = sd.sdMonsters.Where(obj => obj.index == genMonsterIndex).SingleOrDefault();
                var monster = monsterPool.GetPoolableObject(obj => obj.name == sdMonster.name);

                // -> 생성되는 몬스터가 없다면 다시 위로 갑니다!
                if (monster == null)
                {
                    continue;
                }

                var bounds = spawnAreaBounds[sdStage.spawnArea[randIndex]];
                var spawnPosX = Random.Range(-bounds.size.x * 0.5f, bounds.size.x * 0.5f);
                var spawnPosZ = Random.Range(-bounds.size.z * 0.5f, bounds.size.z * 0.5f);
                var centerPos = new Vector3(bounds.center.x, 0, bounds.center.z);

                // -> 몬스터가 생성될 위치에 Ray를 쏴서 그 지점의 Y값을 얻으려는 작업
                transform.position = centerPos + new Vector3(spawnPosX, 50f, spawnPosZ);
                Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 55f, 1 << LayerMask.NameToLayer("Floor"));
                Debug.DrawRay(transform.position, Vector3.down * 55f, Color.red);

                // -> 만약 레이에 닿은 터레인이 있다면 몬스터 생성
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
            // -> 현재 스테이지 정보를 가져옴
            var sdStage = GameManager.User.boStage.sdStage;

            var arrayIndex = -1;

            // -> 현재 스테이지에서 생성되는 몬스터의 인덱스를 지니고 있는 배열의 길이만큼 배열을 돌려
            //    i번째 몬스터 배열에 저장되어있는 인덱스 값과 파라미터로 받은 인덱스값이 같다면 arrayIndex에
            //    i값을 대입하고 반복문을 멈춘다
            for (int i = 0; i < sdStage.genMonsters.Length; i++)
            {
                if (sdStage.genMonsters[i] == monsterIndex)
                {
                    arrayIndex = i;
                    break;
                }
            }

            // -> SdStage에 저장된 spawnArea 배열의 arrayIndex번째 값을 이용하여 Bounds 정보를 가져온다
            var bounds = spawnAreaBounds[sdStage.spawnArea[arrayIndex]];
            var spawnPosX = Random.Range(-bounds.size.x * .5f, bounds.size.x * .5f);
            var spawnPosZ = Random.Range(-bounds.size.z * .5f, bounds.size.z * .5f);

            transform.position = new Vector3(spawnPosX, 50f, spawnPosZ);
            Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 55f, 1 << LayerMask.NameToLayer("Floor"));
            Debug.DrawRay(transform.position, Vector3.down * 55f, Color.red);

            return new Vector3(spawnPosX, hit.point.y, spawnPosZ);
        }

        #endregion
    }
}