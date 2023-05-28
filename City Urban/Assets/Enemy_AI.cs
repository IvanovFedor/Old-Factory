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

    [Header("StepsOfEnemy")]//���������
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
        if (distanse <= 3f && Target.tag == "Point" && Target.tag != "Player")//��������� ����� ��� ���������� �������� ������
        {
            Timer_wait += 0.25f;
            animator.SetInteger("Stage", 0);
            if (Timer_wait >= 50f)
            {
                SearchPoint();
                Timer_wait = 0f;
            }
        } 
        if(distanseLast <= 3f && Target.tag != "Player")//������ ������ ��������
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
        //��� �� ���������! ������ ������
        if (distansePl <= 2f && IsPlKill == false)
        {
            print("����� ������� � ������ ������ ������");
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
    //���� ��������� �����
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


    //������ ����� �� ������� ������
    public void Engry()// ������ ��� �������� / �������������� �������� �����
    {
        PointLast.transform.position = Player_tg.transform.position;
        animator.SetInteger("Stage", 1);
    }

    public void DontEngry()//������� ��������� ������� �� �� ���������� � ���������� ��� � ��������� ������� ������ / ���� ������
    {
        animator.SetInteger("Stage", 1);
        if (StepsOfEnemy == "DontEngry")
        {
            StepsOfEnemy = "WaitIFindYou";
            WaitIFindYou(Player_tg.transform);
        }
    }

    public void WaitIFindYou(Transform LastPosition)//���������� ���������� ���� ����� � ������������ �������
    {
        PointLast.transform.position = LastPosition.position;
        animator.SetInteger("Stage", 1);
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
