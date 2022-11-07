using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    // 랜덤 시간의 범위
    public float minTime = 1;
    public float maxTime = 5;
    // 생성 시간
    float createTime;
    // 경과 시간
    float currnetTime;
    // 드론을 생성할 위치
    public Transform[] spawnPoints;
    // 드론 공장
    public GameObject droneFactory;

    // Start is called before the first frame update
    void Start()
    {
        // 생성 시간을 랜덤 범위에서 설정
        createTime = Random.Range(minTime, maxTime);
    }

    // Update is called once per frame
    void Update()
    {
        // 1. 시간이 흘러야 한다.
        currnetTime += Time.deltaTime;
        // 2. 만약 경과 시간이 생성 시간을 초과했다면
        if(currnetTime > createTime)
        {
            // 3. 드론 생성
            GameObject drone = Instantiate(droneFactory);
            // 4. 드론 위치 설정
            // 랜덤으로 spawnPoints 중 하나를 뽑는다.
            int index = Random.Range(0, spawnPoints.Length);
            // 드론의 위치를 랜덤으로 뽑힌 spawnPoint의 위치로 할당 
            drone.transform.position = spawnPoints[index].position;
            // 5. 경과 시간 초기화
            currnetTime = 0;
            // 6. 생성 시간 재할당
            createTime = Random.Range(minTime, maxTime);
        }
    }
}
