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
    /// 스테이지 세팅 작업들을 수행할 클래스
    /// 스테이지 전환 시 처리 작업을 수행함( 해당 스테이지에 필요한 리소스 로드 및 인스턴스 생성 )
    /// </summary>
    public class StageManager : Singleton<StageManager>
    {
        // public
        public int huntedMon;                       // 사냥한 몬스터 개수
        public bool isMonReady;                     // 몬스터를 생성할 준비가되었는가?
        public Transform monsterHolder;             // 몬스터를 자식으로 가질 부모 홀더

        // private
        private GameObject currentStage;            // 현재 스테이지 객체
        private Transform NPCHolder;                // NPC를 자식으로 가질 부모 홀더
        private float currentMonSpawnTime;          // 현재 몬스터 스폰 시간
        private float maxMonSpawnTime;              // 최대 몬스터 스폰 시간

        /// <summary>
        /// 몬스터 스폰 지역
        /// </summary>
        private Dictionary<int, Bounds> spawnAreaBounds = new Dictionary<int, Bounds>();

        private void Update()
        {
            // 몬스터 생성 준비가 안 되어있다면
            if (!isMonReady)
            {
                return;
            }

            // 몬스터 스폰 시간 업데이트
            CheckSpawnTime();
        }

        /// <summary>
        /// 스테이지를 변경하는 메서드
        /// </summary>
        /// <returns></returns>
        public IEnumerator ChangeStage()
        {
            isMonReady = false;

            var sdStage = GameManager.User.boStage.sdStage;
            var resourceManager = ResourceManager.Instance;
            var objectPoolManager = ObjectPoolManager.Instance;

            // 현재 스테이지가 존재한다면
            if (currentStage != null)
            {
                Destroy(currentStage);
            }

            // 프리팹으로 만들어놓은 스테이지를 가져와 생성
            currentStage = Instantiate(resourceManager.LoadObject(sdStage.resourcePath));

            // 스테이지와 맞는 오디오 소스로 설정
            AudioManager.Instance.ChangeAudioClip(sdStage.resourcePath.Remove(0, sdStage.resourcePath.LastIndexOf('/') + 1));

            // 로딩 씬을 띄워놓고 리소스를 생성하면, 로딩 씬에 리소스가 생기므로 인게임에 옮겨준다
            SceneManager.MoveGameObjectToScene(currentStage, SceneManager.GetSceneByName(SceneType.InGame.ToString()));

            // 이전 스테이지의 몬스터 스폰 지역을 삭제
            spawnAreaBounds.Clear();

            // 이전 스테이지의 몬스터 풀을 삭제
            objectPoolManager.ClearPool<Monster>(PoolType.Monster);

            // 이전 스테이지의 몬스터와 NPC를 삭제
            var battleManager = BattleManager.Instance;
            battleManager.Monsters.Clear();
            battleManager.ClearNPC();

            var sdMonsters = GameManager.SD.sdMonsters;

            // 현재 스테이지에 생성할 몬스터가 존재한다면 풀에 등록하는 작업
            for (int i = 0; i < sdStage.genMonsters.Length; i++)
            {
                var sdMonster = sdMonsters.Where(obj => obj.index == sdStage.genMonsters[i]).SingleOrDefault();

                // 생성할 몬스터가 존재한다면
                if (sdMonster != null)
                {
                    // 몬스터 생성 후 풀에 등록 요청
                    resourceManager.LoadPoolableObject<Monster>(PoolType.Monster, sdMonster.resourcePath, 5);
                }
                else
                {
                    continue;
                }

                // 현재 스테이지의 기획 데이터 안에 있는 스폰 지역 데이터를 가져옴
                var spawnAreaIndex = sdStage.spawnArea[i];

                // 스폰 지역이 존재한다면
                if (spawnAreaIndex != -1)
                {
                    // 스폰 지역을 저장해놓는 Dictionary에 없다면
                    if (!spawnAreaBounds.ContainsKey(spawnAreaIndex))
                    {
                        // 현재 스테이지의 스폰 지역 객체의 하위 객체를 가져옴
                        var spawnArea = currentStage.transform.Find("SpawnPosHolder").GetChild(spawnAreaIndex);

                        // 가져온 하위 객체의 Collider을 Dictionary에 저장
                        spawnAreaBounds.Add(spawnAreaIndex, spawnArea.GetComponent<Collider>().bounds);
                    }
                }
            }

            yield return null;
        }

        /// <summary>
        /// 씬/스테이지 전환 완료 후, 실행할 메서드
        /// </summary>
        public void OnChangeStageComplete()
        {
            // 현재 스테이지에 존재하는 몬스터 체력바를 정리
            UIWindowManager.Instance.GetWindow<UIBattle>().Clear();

            ClearSpawnTime();
            SpawnNPC();
            SpawnCharacter();
            SpawnMonster();

            // 몬스터 생성 준비 완료
            isMonReady = true;
        }

        /// <summary>
        /// 에이전트의 목적지(destPos)를 반환해주는 메서드
        /// </summary>
        /// <param name="monsterIndex"> 생성될 몬스터 인덱스 값 </param>
        /// <returns></returns>
        public Vector3 GetRandPosInArea(int monsterIndex)
        {
            var sdStage = GameManager.User.boStage.sdStage;

            var arrayIndex = -1;

            // 현재 스테이지에 목적지를 받을 몬스터가 존재하는지 확인 작업
            for (int i = 0; i < sdStage.genMonsters.Length; i++)
            {
                // 몬스터가 존재한다면
                if (sdStage.genMonsters[i] == monsterIndex)
                {
                    // 동일 값이 존재하는 위치를 저장
                    arrayIndex = i;
                    break;
                }
            }

            // 저장해놓은 위치 값을 이용해서 스폰 지역을 가져온다
            var bounds = spawnAreaBounds[sdStage.spawnArea[arrayIndex]];

            // 가져온 스폰 지역을 이용해서 새로운 목적지 설정
            var spawnPosX = Random.Range(-bounds.size.x * .5f, bounds.size.x * .5f);
            var spawnPosZ = Random.Range(-bounds.size.z * .5f, bounds.size.z * .5f);

            transform.position = new Vector3(spawnPosX, 50f, spawnPosZ);
            Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 55f, 1 << LayerMask.NameToLayer("Floor"));
            Debug.DrawRay(transform.position, Vector3.down * 55f, Color.red);

            // 목적지 반환
            return new Vector3(spawnPosX, hit.point.y, spawnPosZ);
        }

        /// <summary>
        /// 씬 이동 시, 캐릭터를 생성하는 메서드
        /// 스테이지 이동 시, 캐릭터의 위치를 설정해주는 메서드
        /// </summary>
        private void SpawnCharacter()
        {
            // 컨트롤러를 찾음
            var playerController = FindObjectOfType<PlayerController>();

            if (playerController == null)
            {
                return;
            }

            // 컨트롤러에 캐릭터가 존재한다면
            if (playerController.PlayerCharacter != null)
            {
                // 스테이지 이동이므로 캐릭터의 위치를 재설정
                var warpEntry = currentStage.transform.Find
                    ($"WarpPosHolder/{GameManager.User.boStage.prevStageIndex}/EntryPos").transform;

                // Position 설정
                playerController.PlayerCharacter.transform.position = warpEntry.position;
                playerController.PlayerCharacter.transform.forward = warpEntry.forward;

                // 카메라도 같이 이동
                playerController.cameraController.SetForceStandarView();

                return;
            }

            // 캐릭터를 생성
            var characterObj = Instantiate(ResourceManager.Instance.LoadObject
                (GameManager.User.boCharacter.sdCharacter.resourcePath));
            characterObj.transform.position = GameManager.User.boStage.prevPos;

            // 생성한 캐릭터에 Character 컴포넌트를 추가하고 초기화
            var playerCharacter = characterObj.GetComponent<Character>();
            playerCharacter.Initialize(GameManager.User.boCharacter);

            // 컨트롤러에 캐릭터를 등록
            playerController.Initialize(playerCharacter);

            // 배틀 매니저에 캐릭터 등록
            BattleManager.Instance.AddActor(playerCharacter);
        }

        /// <summary>
        /// NPC 생성 메서드
        /// </summary>
        private void SpawnNPC()
        {
            if (NPCHolder == null)
            {
                NPCHolder = new GameObject("NPCHolder").transform;
                NPCHolder.position = Vector3.zero;
            }

            // 현재 스테이지에 존재하는 NPC정보를 다 가져옴
            var stageIndex = GameManager.User.boStage.sdStage.index;
            var npcs = GameManager.SD.sdNPCs.Where(obj => obj.stageIndex == stageIndex)?.ToList();
            var battleManager = BattleManager.Instance;

            // NPC 개수만큼 생성하는 작업
            for (int i = 0; i < npcs.Count; i++)
            {
                // NPC 프리팹을 가져와서 생성
                var npcObj = Instantiate(ResourceManager.Instance.LoadObject(npcs[i].resourcePath), NPCHolder);
                var npc = npcObj?.GetComponent<NPC>();
                var boNPC = new BoNPC(npcs[i]);

                // NPC 초기화
                npc.Initialize(boNPC);

                // 배틀 매니저에 NPC 등록
                battleManager.AddNPC(npc);
            }
        }

        /// <summary>
        /// 몬스터 스폰 시간을 체크하여 몬스터 생성 여부를 판단하는 메서드
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
        /// 몬스터 스폰 시간 초기화 메서드
        /// </summary>
        private void ClearSpawnTime()
        {
            currentMonSpawnTime = 0;
            maxMonSpawnTime = Random.Range(Spawn.MinMonsterSpawnTime, Spawn.MaxMonsterSpawnTime);
        }

        /// <summary>
        /// 몬스터를 풀에서 가져오는 메서드
        /// </summary>
        private void SpawnMonster()
        {
            if (monsterHolder == null)
            {
                monsterHolder = new GameObject("MonsterHolder").transform;
                monsterHolder.position = Vector3.zero;
            }

            var sd = GameManager.SD;
            var sdStage = GameManager.User.boStage.sdStage;

            // 몬스터 생성 제한 값에 이상이라면
            if (monsterHolder.childCount >= sdStage.stageMonCount)
            {
                return;
            }

            // 생성할 몬스터 개수를 랜덤으로 설정
            var monsterSpawnCnt = Random.Range(Spawn.MinMonsterSpawnCnt, Spawn.MaxMonsterSpawnCnt);

            // 몬스터가 등록 되어있는 풀을 가져옴
            var monsterPool = ObjectPoolManager.Instance.GetPool<Monster>(PoolType.Monster);
            var battleManager = BattleManager.Instance;

            // 개수만큼 몬스터를 생성하는 
            for (int i = 0; i < monsterSpawnCnt; i++)
            {
                // 스테이지에 생성 가능한 몬스터의 배열 길이를 이용해서 랜덤 값 생성
                var randIndex = Random.Range(0, sdStage.genMonsters.Length);

                // 랜덤 값으로 배열에 있는 몬스터 인덱스를 가져옴
                var genMonsterIndex = sdStage.genMonsters[randIndex];

                // 생성할 몬스터가 없다면
                if (genMonsterIndex == -1)
                {
                    return;
                }

                // 몬스터 인덱스가 보스 인덱스라면
                if (genMonsterIndex == StaticData.BossIndex)
                {
                    if (huntedMon >= StaticData.BossSpawn)
                    {
                        huntedMon = 0;
                    }
                    else
                    {
                        continue;
                    }
                }

                // 생성할 몬스터의 기획 데이터를 가져옴
                var sdMonster = sd.sdMonsters.Where(obj => obj.index == genMonsterIndex).SingleOrDefault();

                // 풀에서 몬스터를 가져옴
                var monster = monsterPool.GetPoolableObject(obj => obj.name == sdMonster.name);

                if (monster == null)
                {
                    continue;
                }

                // 스폰 지역 Dictionary에서 몬스터 스폰 지역을 가져옴
                var bounds = spawnAreaBounds[sdStage.spawnArea[randIndex]];

                // 가져온 스폰 지역을 이용해서 스폰 지역 설정
                var spawnPosX = Random.Range(-bounds.size.x * 0.5f, bounds.size.x * 0.5f);
                var spawnPosZ = Random.Range(-bounds.size.z * 0.5f, bounds.size.z * 0.5f);
                var centerPos = new Vector3(bounds.center.x, 0, bounds.center.z);

                // 스폰 지역에 레이를 발사하여 스폰 지역의 Y값을 가져옴
                transform.position = centerPos + new Vector3(spawnPosX, 50f, spawnPosZ);
                Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 55f, 1 << LayerMask.NameToLayer("Floor"));
                Debug.DrawRay(transform.position, Vector3.down * 55f, Color.red);

                // 스폰할 몬스터 초기화
                monster.transform.position = centerPos + new Vector3(spawnPosX, hit.point.y, spawnPosZ);
                monster.transform.SetParent(monsterHolder, true);
                monster.Initialize(new BoMonster(sdMonster));
                monster.State = Define.Actor.ActorState.None;
                battleManager.AddActor(monster);
            }
        }
    }
}