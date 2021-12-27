using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.Define
{
    public enum SceneType { None, Title, StartLoading, NovelGame, Loading, InGame }

    public enum IntroPhase { None, Start, ApplicationSetting, Server, StaticData, UserData, Resource, UI, Complete }

    public enum PoolType { None, Character, Monster, Item, MonHpBar, DialogueButton, QuestSlot }

    public enum ItemType { None, Expendables, Equipment, Quest, Etc }

    public enum QuestType { None, Collection, Hunt, Conversation }

    public enum UIType { None, HpBar, EnergyBar }

    public class Camera
    {
        public enum CamView { Standard, Front }
        public const float CamRotSpeed = 3f;
        public const string CamPosPath = "Prefabs/CamPos";
    }

    public class Input
    {
        public const string AxisX = "Horizontal";
        public const string AxisZ = "Vertical";
        public const string MouseX = "Mouse X";
        public const string MouseY = "Mouse Y";
        public const string FrontCam = "Fire3";
        public const string Jump = "Jump";
        public const string MouseLeft = "Fire1";
        public const string MouseRight = "Fire2";
        public const string Inventory = "Inventory";
        public const string Quest = "Quest";
        public const string FormChange = "FormChange";
        public const string Interaction = "Interaction";
        public const string Sprint = "Sprint";
    }

    public class Spawn
    {
        public const int MinMonsterSpawnCnt = 1;
        public const int MaxMonsterSpawnCnt = 3;
        public const float MinMonsterSpawnTime = 10f;
        public const float MaxMonsterSpawnTime = 20f;
    }


    public class Actor
    {
        public enum ActorType { None, Character, Monster, Form, Boss }

        public enum ActorState { None, Idle, Walk, Sit, Rise, Jump, Attack, Dead, InWeapon, OutWeapon, Damage }

        public enum AttackType { None, Normal, Boss }

        public enum CharType { None, NovelChar, NPC, Narration }

        public enum NPCType { None, Store, Normal }

        public class CharAnimParam
        {
            public int isWalk;
            public int isJump;
            public int isSit;
            public int isRise;
            public int isAttack;
            public int randAttack;
            public int isDead;
            public int InWeapon;
            public int OutWeapon;
            public int isGround;
            public int isDamage;

            public CharAnimParam()
            {
                isWalk = Animator.StringToHash("isWalk");
                isJump = Animator.StringToHash("isJump");
                isSit = Animator.StringToHash("isSit");
                isRise = Animator.StringToHash("isRise");
                isAttack = Animator.StringToHash("isAttack");
                randAttack = Animator.StringToHash("randAttack");
                isDead = Animator.StringToHash("isDead");
                InWeapon = Animator.StringToHash("InWeapon");
                OutWeapon = Animator.StringToHash("OutWeapon");
                isGround = Animator.StringToHash("isGround");
                isDamage = Animator.StringToHash("isDamage");
            }
        }

        public class MonAnimParam
        {
            public int isWalk;
            public int isAttack;
            public int isDead;
            public int isDamage;
            public int randAttack;

            public MonAnimParam()
            {
                isWalk = Animator.StringToHash("isWalk");
                isAttack = Animator.StringToHash("isAttack");
                isDead = Animator.StringToHash("isDead");
                isDamage = Animator.StringToHash("isDamage");
                randAttack = Animator.StringToHash("randAttack");
            }
        }
    }

    public class Novel
    {
        public const int firstNovelIndex = 1000;
        public const int nextStageLoadIndex = 1076;
    }

    public class Monster
    {
        public const float MinPatrolWaitTime = 1f;
        public const float MaxPatrolWaitTime = 3f;
    }

    public class StaticData
    {
        public const string SDPath = "Assets/StaticData";
        public const string SDExcelPath = "Assets/StaticData/Excel";
        public const string SDJosnPath = "Assets/StaticData/Json";
        public const float BaseNovelSize = 0.7915039f;
        public const float NPCNovelSize = 1.518182f;
        public const float ChangeFormValue = 20.0f;
        public const float BaseSpeed = 0.5f;
        public const int BossIndex = 1002;
    }

    public class ItemData
    {
        public const KeyCode startNumer = KeyCode.Alpha1;
        public const KeyCode endNumber = KeyCode.Alpha9;
        public const int interval = 48;
    }

    public class Resource
    {
        public enum AtlasType
        {
            None,
            BackGround,
            SchoolImage,
            Portrait,
            UIAtlase,
            ItemAtlase
        }
    }

    public class Quest
    {
        public enum QuestWindow { None, Order, List, Content }
        public enum QuestTab { None, Progress, Completed }
        public enum QuestOrderTab { NoProgress, Progress, Clear }
    }
}