using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_AI : MonoBehaviour
{
    public GameObject Player_tg;
    public NavMeshAgent nav;
    public GameObject[] Points;
    public GameObject Target;

    public float Speed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        SearchPoint();
        nav.speed = Speed;
    }

    // Update is called once per frame
    void Update()
    {
        //Проверка на IQ бота
        float distanse = Vector3.Distance(gameObject.transform.position, Target.transform.position);
        if (distanse < 1f)
        {
            SearchPoint();
        }
        //УУУУУУ тебе пипец!!!
        nav.destination = Target.transform.position;
        //ТУТ НЕ ДОРАБОТКА! Смерть игрока
        if(distanse < 1 && Target.tag == "Player")
        {
            print("нужен переход к методу смерти игрока");
        }
    }

    void SearchPoint()
    {
        foreach(GameObject target_point in Points)
        {
            float distanse = Vector3.Distance(gameObject.transform.position, target_point.transform.position);
            if(distanse > 50)
            {
                Target = target_point;
            }
        }
    }

    public void GetSomeEvents(string Event, float SomeElements)
    {
        switch (Event)
        {
            //Получаем скорость по сложности или по настройке
            case "Speed":
                Speed = SomeElements;
                break;
            //Палит игрока по звуку
            case "Wait! Sound":
                break;
            //Какой либо тип дамага или дебафа
            case "Damage":
                break;
        }
    }
}
