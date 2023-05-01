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

    [Header("StepsOfEnemy")]//���������
    [SerializeField] private string StepsOfEnemy = "Idle";

    [Header("Parametors")]
    [SerializeField] private float Speed = 3f;

    [SerializeField] private float Timer_wait = 0f;

    void Start()
    {
        Points = GameObject.FindGameObjectsWithTag("Point");//���������� �����
        nav.speed = Speed;//���������� ��������
        Player_tg = GameObject.FindGameObjectWithTag("Player");//���������� ������
        SearchPoint();//���� �����
    }


    void Update()
    {
        //���������� ��� ��������� ��������� 
        float distanse = Vector3.Distance(gameObject.transform.position, Target.transform.position);
        float distanseLast = Vector3.Distance(gameObject.transform.position, PointLast.transform.position);
        if (distanse <= 2f && Target.tag == "Point")//��������� ����� ��� ���������� �������� ������
        {
            Timer_wait += 0.25f;
            if (Timer_wait >= 50f)
            {
                SearchPoint();
                Timer_wait = 0f;
            }
        } 
        if(distanseLast <= 2f)//������ ������ ��������
        {
            Timer_wait += 0.25f;
            if (Timer_wait >= 50f)
            {
                SearchPoint();
                StepsOfEnemy = "Idle";
                Timer_wait = 0f;
            }
        }
        //������ � ������ ��� ������
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
        
        //��� �� ���������! ������ ������
        if (distanse < 1f && Target.tag == "Player")
        {
            print("����� ������� � ������ ������ ������");
        }


        //������ ��� � ������
        float distansePl = Vector3.Distance(gameObject.transform.position, Player_tg.transform.position);
        PointRay.transform.LookAt(Player_tg.transform.position);//������������ ����� � ������
        Ray rayToPlayer = new Ray(PointRay.transform.position, PointRay.transform.forward * 200f);//������ ��� �� ������ ����� � ������
        Debug.DrawRay(PointRay.transform.position, PointRay.transform.forward * 200f, Color.red);
        RaycastHit Hit;//���������� ������� ���� � ���������
        if (Physics.Raycast(rayToPlayer, out Hit) && distansePl <= 20f)//���� ��� ��������� ���� � ��� ����� � ��������� <= 20 ������ �� ��� ��� �� ������
        {
            if (Hit.collider.tag == "Player")
            {
                Engry();
                StepsOfEnemy = "GoPlayer";//������ ��������� �� ����������
            }
        }else if (distansePl >= 30f && StepsOfEnemy == "GoPlayer")//���� ��� �� �������� �� ������ � ���� ��������� � 30 ������ �� ������� �� ������ ��� � ��������� ����� ��� ��� ������� �����
        {
            if (Hit.collider.tag != "Player")
            {
                StepsOfEnemy = "DontEngry";//������ ��������� �� �� �����������
                DontEngry();
            }
        }
    }
    //���� ��������� �����
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


    //������ ����� �� ������� ������
    public void Engry()// ������ ��� �������� / �������������� �������� �����
    {
        PointLast.transform.position = Player_tg.transform.position;
    }

    public void DontEngry()//������� ��������� ������� �� �� ���������� � ���������� ��� � ��������� ������� ������ / ���� ������
    {
        if(StepsOfEnemy == "DontEngry")
        {
            StepsOfEnemy = "WaitIFindYou";
            WaitIFindYou(Player_tg.transform);
        }
    }

    public void WaitIFindYou(Transform LastPosition)//���������� ���������� ���� ����� � ������������ �������
    {
        PointLast.transform.position = LastPosition.position;
    }

    public void SetSomeEvents(string Event, float SomeElements = 0)// ������� �������� ����������
    {
        switch (Event)//����� ��������
        {
            //�������� �������� �� ��������� ��� �� ���������
            case "Speed":
                Speed = SomeElements;
                nav.speed = Speed;
                break;
            //����� ������ �� �����
            case "Wait! Sound":
                Engry();
                StepsOfEnemy = "GoPlayer";
                break;
            //����� ������ ����� ��� �����
            case "Wait! I see You":
                Engry();
                break;
            //����� ���� ��� ������ ��� ������
            case "Damage":
                break;
        }
    }
}
