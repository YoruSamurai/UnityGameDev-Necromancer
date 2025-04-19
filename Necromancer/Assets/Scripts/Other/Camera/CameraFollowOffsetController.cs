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
        if (Input.GetKeyDown(KeyCode.D) && !isHolding)
        {
            StopAllCoroutines();
            isHolding = true;
            holdingDir = 1;
            holdTime = 0;
            if (lastTimeDir == -1)
                transposer.m_TrackedObjectOffset.x *= -1f;
            lastTimeDir = 1;
        }
        else if (Input.GetKeyDown(KeyCode.A) && !isHolding)
        {
            StopAllCoroutines();
            isHolding = true;
            holdingDir = -1;
            holdTime = 0;
            if (lastTimeDir == 1)
                transposer.m_TrackedObjectOffset.x *= -1f;
            lastTimeDir = -1;
        }

        if (isHolding && Input.GetKey(KeyCode.D) && holdingDir == 1)
        {
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
        else if (isHolding && Input.GetKey(KeyCode.A) && holdingDir == -1)
        {
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

        if (isHolding && Input.GetKeyUp(KeyCode.D) && holdingDir == 1)
        {

                StartCoroutine(SmoothOffsetTo(5f, holdTime));
            /*if (Mathf.Abs(transform.position.x - player.transform.position.x) < 3f)
            {

            }
            else if (transform.position.x - player.transform.position.x > 2f)
            {
                Debug.Log("e");
            }
            else
            {
                StartCoroutine(SmoothOffsetTo(0f, holdTime));

            }*/

            holdTime = 0;
            isHolding = false;
        }
        else if (isHolding && Input.GetKeyUp(KeyCode.A) && holdingDir == -1)
        {

                StartCoroutine(SmoothOffsetTo(5f, holdTime));
            /*if (Mathf.Abs(transform.position.x - player.transform.position.x) < 3f)
            {

            }
            else if (player.transform.position.x - transform.position.x > 2f)
            {
                Debug.Log("e");
            }
            else
            {
                StartCoroutine(SmoothOffsetTo(0f, holdTime));
            }*/

            holdTime = 0;
            isHolding = false;
        }

    }

    private IEnumerator SmoothOffsetTo(float target, float duration)
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
            start += Time.deltaTime * 10;
            
            if (start < target)
                transposer.m_TrackedObjectOffset.x = start;
            yield return null;
        }


    }
}
