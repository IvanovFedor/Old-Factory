using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_AI : MonoBehaviour
{
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
        if (distanse <= 2f && Target.tag == "Point")//следующий поинт при достижении целевого поинта
        {
            Timer_wait += 0.25f;
            if (Timer_wait >= 50f)
            {
                SearchPoint();
                Timer_wait = 0f;
            }
        } 
        if(distanseLast <= 2f)//Потеря игрока монстром
        {
            Timer_wait += 0.25f;
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
        
        //ТУТ НЕ ДОРАБОТКА! Смерть игрока
        if (distanse < 1f && Target.tag == "Player")
        {
            print("нужен переход к методу смерти игрока");
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
    }
    //ищет рандомный поинт
    void SearchPoint()
    {
        foreach(GameObject target_point in Points)
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
    }

    public void DontEngry()//сменяет поведения монстра на не агресивную и направляет его к последней позиции игрока / ищет игрока
    {
        if(StepsOfEnemy == "DontEngry")
        {
            StepsOfEnemy = "WaitIFindYou";
            WaitIFindYou(Player_tg.transform);
        }
    }

    public void WaitIFindYou(Transform LastPosition)//присвавает получаемый ласт поинт к действуещему объекту
    {
        PointLast.transform.position = LastPosition.position;
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
}
