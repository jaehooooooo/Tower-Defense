using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TeleportStraight : MonoBehaviour
{
    // 텔레포트를 표시할 UI
    public Transform teleportCircleUI;
    // 선을 그릴 라인 렌더러
    LineRenderer lr;

    // 최초 텔레포트 UI의 크기
    Vector3 originScale = Vector3.one * 0.02f;

    // 위프 사용 여부
    public bool isWarp = false;
    // 위프에 걸리는 시간
    public float warpTime = 0.1f;
    // 사용하고 있는 포스트 프로세싱 불륨 컴포넌트
    public PostProcessVolume post;

    // Start is called before the first frame update
    void Start()
    {
        // 시작할 때 비활성화한다
        teleportCircleUI.gameObject.SetActive(false);
        // 라인 렌더러 컴포넌트 얻어오기
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // 버튼의 입력이 GetDone -> GetUP -> Get 함수 순으로 검출 될 수 있도록 순서 바꾸기
        // 왼쪽 컨트롤러의 One 버튼을 누르면
        if(ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 라인 렌더러 컴포넌트 비활성화
            lr.enabled = true;
        }
        // 왼쪽 컨트롤러의 one 버튼에서 손을 떼면
        else if(ARAVRInput.GetUp(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 라인 렌더러 비활성화
            lr.enabled = false;
            // 플레이어 텔레포트 
            // 주의 : Character Controller 컴포넌트가 플레이어 움직임을 담당하고 있기 때문에 transform으로 이동시킬때 제약이 있을 수 있음
            // => Character Controller 컴토넌트 비활 -> 이동 끝나면 활성화
            if (teleportCircleUI.gameObject.activeSelf)
            {
                // 워프 기능 사용이 아닐때 순간 이동 처리
                if(!isWarp)
                {
                    GetComponent<CharacterController>().enabled = false;
                    // 텔레포트 UI 위치로 순간이동
                    transform.position = teleportCircleUI.position + Vector3.up;
                    GetComponent<CharacterController>().enabled = true;
                }
                else
                {
                    // 워프 기능을 사용할때는 Warp()코루틴 호출
                    StartCoroutine(Warp());
                }
            }
            // 텔레포트 UI 비활성화
            teleportCircleUI.gameObject.SetActive(false);
        }
        // 왼쪽 컨트롤러의 One 버튼을 누르고 있을 때
        else if(ARAVRInput.Get(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 텔레포트 UI 그리기
            // 1. 왼쪽 컨트롤러를 기준으로 Ray를 만든다
            Ray ray = new Ray(ARAVRInput.LHandPosition, ARAVRInput.LHandDirection);
            RaycastHit hitInfo;
            int layer = 1 << LayerMask.NameToLayer("Terrain");
            // 2. Terrain만 Ray 충돌 검출
            if (Physics.Raycast(ray, out hitInfo, 200, layer))
            {
                // 부딪힌 지점에 텔레포트 UI 표시
                // 3. Ray가 부딪힌 지점에 라인 그리기
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, hitInfo.point);
                // 4. Ray가 부딪힌 지점에 텔레포트 UI 표시
                teleportCircleUI.gameObject.SetActive(true);
                teleportCircleUI.position = hitInfo.point;
                // 텔레포트 UI가 위로 누워있도록 방향 설정
                teleportCircleUI.forward = hitInfo.normal;
                // 텔레포트 UI의 크기가 거리에 따라 보정되도록 설정
                teleportCircleUI.localScale = originScale * Mathf.Max(1, hitInfo.distance);
            }
            else
            {
                // Ray 충돌이 발생하지 않으면 선이 Ray방향으로 그려지도록 처리
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, ray.origin + ARAVRInput.LHandDirection * 200);
                // 텔레포트 UI는 화면에서 비활성화
                teleportCircleUI.gameObject.SetActive(false);
                
            }
        }
    }

    IEnumerator Warp()
    {
        // 워프 느낌을 표현할 모션블러
        MotionBlur blur;
        // 워프 시작점 기억
        Vector3 pos = transform.position;
        // 목적지
        Vector3 targetPos = teleportCircleUI.position + Vector3.up;
        // 워프 경과 시간
        float currentTime = 0;
        // 포스트 프로세싱에서 사용중인 프로파일에서 모션블러 얻어오기
        post.profile.TryGetSettings<MotionBlur>(out blur);
        // 워프 시작 전 블러 켜기
        blur.active = true;
        GetComponent<CharacterController>().enabled = false;

        // 경과 시간이 워프 시간보다 짧은 시간 동안 이동 처리
        while(currentTime < warpTime)
        {
            // 경과시간 흐르게 하기
            currentTime += Time.deltaTime;
            // 워프의 시작점에서 도착점에 도착하기 위해 워프 시간 동안 이동
            transform.position = Vector3.Lerp(pos, targetPos, currentTime / warpTime);
            // 코루틴 대기
            yield return null;
        }
        // 텔레포트 UI위치로 순간 이동
        transform.position = teleportCircleUI.position + Vector3.up;
        // 캐릭터 컨트롤러 다시 켜기
        GetComponent<CharacterController>().enabled = true;
        // 포스트 효과 끄기
        blur.active = false;
    }
}
