using Cinemachine;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CameraFollowOffsetController : MonoBehaviour
{
    [SerializeField] private Player player;

    [Header("虚拟摄像机")]
    public CinemachineVirtualCamera virtualCamera;

    [Header("左右偏移量")]
    public float offsetX = 3f;

    private CinemachineFramingTransposer transposer;
    private float targetOffsetX = 0;
    private float targetOffsetY;


    private float holdTime = 0;
    private bool isHolding = false;
    private int holdingDir; //1D -1A
    private int lastTimeDir = 1;
    private bool isCoroutineRunning = false; // 新增的布尔变量

    private const string TweenId = "CameraOffsetX";


    private void Start()
    {
        if (virtualCamera != null)
        {
            transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            transposer.m_TrackedObjectOffset.x = 0;
        }
    }

    private void Update()
    {
        if (transposer == null) return;
        if (Input.GetKeyDown(KeyCode.D))
        {
            StopAllCoroutines();
            isCoroutineRunning = false;
            if (lastTimeDir == -1)
                transposer.m_TrackedObjectOffset.x *= -1f;
            lastTimeDir = 1;
            if (player.stateMachine.currentState == player.climbState)
            {
                StartCoroutine(SmoothOffsetTo(5f, 2f,2));
                isCoroutineRunning = true;
                return;
            }
            Debug.Log("协程被停职");
            isHolding = true;
            holdingDir = 1;
            holdTime = 0;
            
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            StopAllCoroutines();
            isCoroutineRunning = false;
            if (lastTimeDir == 1)
                transposer.m_TrackedObjectOffset.x *= -1f;
            lastTimeDir = -1;
            if (player.stateMachine.currentState == player.climbState)
            {
                StartCoroutine(SmoothOffsetTo(5f, 2f,2));
                isCoroutineRunning = true;
                return;
            }
            Debug.Log("协程被停职");

            isHolding = true;
            holdingDir = -1;
            holdTime = 0;
            
        }

        if (isHolding && Input.GetKey(KeyCode.D) && holdingDir == 1)
        {
            bool isWallDetected = player.IsWallBodyDetected();
            if (isWallDetected && isCoroutineRunning == false)
            {
                StartCoroutine(SmoothOffsetTo(5f, 2f, 1));
                isCoroutineRunning=true;
            }
            else
            {
                if (isWallDetected)
                {
                    return;
                }
                holdTime += Time.deltaTime;
                if (transposer.m_TrackedObjectOffset.x > 0)
                {
                    transposer.m_TrackedObjectOffset.x -= Time.deltaTime * 5;
                }
                else if (transposer.m_TrackedObjectOffset.x < 0)
                {
                    transposer.m_TrackedObjectOffset.x += Time.deltaTime * 5;
                }
            }
        }
        else if (isHolding && Input.GetKey(KeyCode.A) && holdingDir == -1)
        {
            bool isWallDetected = player.IsWallBodyDetected();
            if (isWallDetected && isCoroutineRunning == false)
            {
                StartCoroutine(SmoothOffsetTo(5f, 2f,1));
                isCoroutineRunning = true;
            }
            else
            {
                if (isWallDetected)
                {
                    return;
                }
                holdTime += Time.deltaTime;
                if (transposer.m_TrackedObjectOffset.x > 0)
                {
                    transposer.m_TrackedObjectOffset.x -= Time.deltaTime * 5;
                }
                else if (transposer.m_TrackedObjectOffset.x < 0)
                {
                    transposer.m_TrackedObjectOffset.x += Time.deltaTime * 5;
                }
            }
        }

        if (isHolding && Input.GetKeyUp(KeyCode.D) && holdingDir == 1)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothOffsetTo(5f, holdTime,1));
            isCoroutineRunning = true;
            holdTime = 0;
            holdingDir = 0;
            isHolding = false;
        }
        else if (isHolding && Input.GetKeyUp(KeyCode.A) && holdingDir == -1)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothOffsetTo(5f, holdTime, 1));
            isCoroutineRunning = true;
            holdingDir = 0;
            holdTime = 0;
            isHolding = false;
        }

    }

    private IEnumerator SmoothOffsetTo(float target, float duration,int speed)
    {
        Debug.Log("开始协程" + "目标" + target + "时间" + duration);
        float start = transposer.m_TrackedObjectOffset.x;
        float timer = 0f;
        if(duration < .3f)
        {
            duration = .5f ;
        }
        while (timer < duration)
        {
            timer += Time.deltaTime;
            if (timer < .1f)
            {
                start += Time.deltaTime * timer * 100 * speed;

            }
            else if(timer + .1f  > duration)
            {
                start += Time.deltaTime * (duration - timer) * 100 * speed;
            }
            else
            {
                start += Time.deltaTime * 10 * speed;

            }
            
            if (start < target)
                transposer.m_TrackedObjectOffset.x = start;
            yield return null;
        }


    }
}
