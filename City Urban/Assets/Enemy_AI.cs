using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_AI : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [Header("Screamer")]
    [SerializeField] private GameObject PlayerCam;
    [SerializeField] private GameObject ScreemerCam;
    [SerializeField] private GameObject DeathCam;
    [SerializeField] private GameObject LocationDeath;

    [Header("Navigation")]
    [SerializeField] private GameObject Player_tg;
    [SerializeField] private NavMeshAgent nav;
    [SerializeField] private GameObject[] Points;
    [SerializeField] private GameObject Target;

    [Header("RayCast")]
    [SerializeField] private GameObject PointRay;

    [Header("LastPoint")]
    [SerializeField] private GameObject PointLast;

    [Header("StepsOfEnemy")]//поведение
    [SerializeField] private string StepsOfEnemy = "Idle";

    [Header("Parametors")]
    [SerializeField] private float Speed = 3f;

    [SerializeField] private float Timer_wait = 0f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip_Screamer;

    [Header("Logic")]
    [SerializeField] private bool IsPlKill = false;

    [Header("PlayerUIDeath")]
    [SerializeField] private GameObject HideObj;
    [SerializeField] private GameObject VisibleObj;


    void Start()
    {
        Points = GameObject.FindGameObjectsWithTag("Point");//обозначаем поинт
        nav.speed = Speed;//обозначаем скорость
        Player_tg = GameObject.FindGameObjectWithTag("Player");//обозначаем игрока
        SearchPoint();//ищем поинт
    }


    void Update()
    {
        //Переменные для измерения растояния 
        float distanse = Vector3.Distance(gameObject.transform.position, Target.transform.position);
        float distanseLast = Vector3.Distance(gameObject.transform.position, PointLast.transform.position);
        if (distanse <= 3f && Target.tag == "Point" && Target.tag != "Player")//следующий поинт при достижении целевого поинта
        {
            Timer_wait += 0.25f;
            animator.SetInteger("Stage", 0);
            if (Timer_wait >= 50f)
            {
                SearchPoint();
                Timer_wait = 0f;
            }
        } 
        if(distanseLast <= 3f && Target.tag != "Player")//Потеря игрока монстром
        {
            Timer_wait += 0.25f;
            animator.SetInteger("Stage", 0);
            if (Timer_wait >= 50f)
            {
                SearchPoint();
                StepsOfEnemy = "Idle";
                Timer_wait = 0f;
            }
        }
        //Ходьба к поинту или игроку
        if (StepsOfEnemy == "Idle")
        {
            nav.destination = Target.transform.position;
        }
        if(StepsOfEnemy == "WaitIFindYou")
        {
            nav.destination = PointLast.transform.position;
        }
        if(StepsOfEnemy == "GoPlayer")
        {
            nav.destination = Player_tg.transform.position;
        }
        
        


        //Кидаем луч к игроку
        float distansePl = Vector3.Distance(gameObject.transform.position, Player_tg.transform.position);
        PointRay.transform.LookAt(Player_tg.transform.position);//поворачиваем поинт к игроку
        Ray rayToPlayer = new Ray(PointRay.transform.position, PointRay.transform.forward * 200f);//кидаем луч из поинта вперёд к игроку
        Debug.DrawRay(PointRay.transform.position, PointRay.transform.forward * 200f, Color.red);
        RaycastHit Hit;//переменная косания луча с объектами
        if (Physics.Raycast(rayToPlayer, out Hit) && distansePl <= 20f)//если луч достигает цели и это игрок и растояние <= 20 метрам то идёт агр на игрока
        {
            if (Hit.collider.tag == "Player")
            {
                Engry();
                StepsOfEnemy = "GoPlayer";//меняем повидение на агресивное
            }
        }else if (distansePl >= 30f && StepsOfEnemy == "GoPlayer")//если луч не попадает по игроку и ирок находится в 30 метрах от монстра то монстр идёт к последней точке где был замечен игрок
        {
            if (Hit.collider.tag != "Player")
            {
                StepsOfEnemy = "DontEngry";//меняем поведение на не агрессивное
                DontEngry();
            }
        }
        //ТУТ НЕ ДОРАБОТКА! Смерть игрока
        if (distansePl <= 2f && IsPlKill == false)
        {
            print("нужен переход к методу смерти игрока");
            StartCoroutine(KillPlayer());
            IsPlKill = true;
        }
        if(IsPlKill == true)
        {
            PlayerCam.transform.parent = null;
            PlayerCam.transform.position = ScreemerCam.transform.position;
            PlayerCam.transform.rotation = ScreemerCam.transform.rotation;
            PlayerCam.GetComponent<Camera>().fieldOfView = 110f;
            Player_tg.SetActive(false);
            animator.SetInteger("Stage", 6);
        }
    }
    //ищет рандомный поинт
    void SearchPoint()
    {
        animator.SetInteger("Stage", 1);
        foreach (GameObject target_point in Points)
        {
            float distanse = Vector3.Distance(gameObject.transform.position, target_point.transform.position);
            if(distanse > 5f)
            {
                int RandomPoint = Random.RandomRange(0, Points.Length);
                Target = Points[RandomPoint];
            }
        }
    }


    //Ставит поинт на позицию игрока
    public void Engry()// Только для удобства / перенаправляет внимание врага
    {
        PointLast.transform.position = Player_tg.transform.position;
        animator.SetInteger("Stage", 1);
    }

    public void DontEngry()//сменяет поведения монстра на не агресивную и направляет его к последней позиции игрока / ищет игрока
    {
        animator.SetInteger("Stage", 1);
        if (StepsOfEnemy == "DontEngry")
        {
            StepsOfEnemy = "WaitIFindYou";
            WaitIFindYou(Player_tg.transform);
        }
    }

    public void WaitIFindYou(Transform LastPosition)//присвавает получаемый ласт поинт к действуещему объекту
    {
        PointLast.transform.position = LastPosition.position;
        animator.SetInteger("Stage", 1);
    }

    public void SetSomeEvents(string Event, float SomeElements = 0)// немного входящих Параметров
    {
        switch (Event)//выбор действия
        {
            //Получаем скорость по сложности или по настройке
            case "Speed":
                Speed = SomeElements;
                nav.speed = Speed;
                break;
            //Палит игрока по звуку
            case "Wait! Sound":
                Engry();
                StepsOfEnemy = "GoPlayer";
                break;
            //Палит игрока когда его видит
            case "Wait! I see You":
                Engry();
                break;
            //Какой либо тип дамага или дебафа
            case "Damage":
                break;
        }
    }


    IEnumerator KillPlayer()
    {
        audioSource.PlayOneShot(audioClip_Screamer);
        yield return new WaitForSeconds(1.12f);
        audioSource.Stop();
        PlayerCam.SetActive(false);
        DeathCam.SetActive(true);
        HideObj.SetActive(false);
        VisibleObj.SetActive(true);
        LocationDeath.SetActive(true);
    }
}
