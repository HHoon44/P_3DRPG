using ProjectChan.Object;
using ProjectChan.UI;
using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.Battle
{
    using ActorType = Define.Actor.ActorType;

    /// <summary>
    /// => 인게임에서 활성화된 액터 객체들을 관리하는 클래스
    /// </summary>
    public class BattleManager : Singleton<BattleManager>
    {
        public List<Actor> Characters { get; private set; } = new List<Actor>();
        public List<Actor> Monsters { get; private set; } = new List<Actor>();
        public List<NPC> NPCs { get; private set; } = new List<NPC>();

        /// <summary>
        /// => 활성화된 액터들을 배틀매니저에 등록하는 메서드
        /// => 리스트에 등록된 액터만 업데이트가 된다
        /// </summary>
        /// <param name="actor"></param>
        public void AddActor(Actor actor)
        {
            // -> 파라미터로 받은 액터안의 액터타입에 접근하여 
            //    액터 타입에 따라 리스트에 저장함
            switch (actor)
            {
                case var character when actor.boActor.actorType == ActorType.Character:
                    Characters.Add(character);
                    break;

                case var monster when actor.boActor.actorType == ActorType.Monster:
                    UIWindowManager.Instance.GetWindow<UIBattle>().AddMonHpBar(actor);
                    Monsters.Add(monster);
                    break;

            }

            actor.gameObject.SetActive(true);
        }

        public void AddNPC(NPC npc)
        {
            NPCs.Add(npc);
        }

        /// <summary>
        /// => 특정 객체의 클래스 안에서 Update 콜백메서드를 직접적으로 갖는 것보다
        ///    특정 객체들을 담을 컨테이너를 만들고, 해당 컨테이너에 객체들을 하나의 
        ///    업데이트 콜백메서드에서 처리하는 것이 성능면에서 우수하다
        /// </summary>
        private void FixedUpdate()
        {
            ActorUpdate(Characters);
            ActorUpdate(Monsters);
            NPCUpdate();
        }

        /// <summary>
        /// 저장된 액터들이 가지고있는 업데이트 함수를 활성화 하는 메서드
        /// </summary>
        /// <param name="actors"> 배틀매니저에 저장되어있는 컨테이너 </param>
        private void ActorUpdate(List<Actor> actors)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                // -> 액터가 죽지 않았다면 업데이트
                if (actors[i].State != Define.Actor.ActorState.Dead)// 수정
                {
                    actors[i].ActorUpdate();
                }
                // -> 액터가 죽었다면 컨테이너에서 제거
                else
                {
                    actors.RemoveAt(i);
                    i--;
                    //-> 반복되는 곳에서 리스트 안에 원소를 지울 땐
                    //   증감 연산자 ++로 사용할 현재 인덱스에서 --를 해줘야 다음 원소로 넘어간다
                }
            }
        }

        /// <summary>
        /// => NPC 스크립트의 업데이트문을 배틀매니저에서 실행하는 메서드
        /// </summary>
        private void NPCUpdate()
        {
            for (int i = 0; i < NPCs.Count; i++)
            {
                if (NPCs[i] != null)
                {
                    // -> NPC 업데이트 돌리기
                    NPCs[i].NPCUpdate();
                }
                else
                {
                    NPCs.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// => 배틀매니저에 저장되어있는 NPC들을 초기화하는 메서드
        /// </summary>
        public void ClearNPC()
        {
            for (int i = 0; i < NPCs.Count; i++)
            {
                if (NPCs[i] != null)
                {
                    Destroy(NPCs[i].gameObject);
                }
            }

            NPCs.Clear();
        }
    }
}
