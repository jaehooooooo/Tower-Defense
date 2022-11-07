using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    // ���� �ð��� ����
    public float minTime = 1;
    public float maxTime = 5;
    // ���� �ð�
    float createTime;
    // ��� �ð�
    float currnetTime;
    // ����� ������ ��ġ
    public Transform[] spawnPoints;
    // ��� ����
    public GameObject droneFactory;

    // Start is called before the first frame update
    void Start()
    {
        // ���� �ð��� ���� �������� ����
        createTime = Random.Range(minTime, maxTime);
    }

    // Update is called once per frame
    void Update()
    {
        // 1. �ð��� �귯�� �Ѵ�.
        currnetTime += Time.deltaTime;
        // 2. ���� ��� �ð��� ���� �ð��� �ʰ��ߴٸ�
        if(currnetTime > createTime)
        {
            // 3. ��� ����
            GameObject drone = Instantiate(droneFactory);
            // 4. ��� ��ġ ����
            // �������� spawnPoints �� �ϳ��� �̴´�.
            int index = Random.Range(0, spawnPoints.Length);
            // ����� ��ġ�� �������� ���� spawnPoint�� ��ġ�� �Ҵ� 
            drone.transform.position = spawnPoints[index].position;
            // 5. ��� �ð� �ʱ�ȭ
            currnetTime = 0;
            // 6. ���� �ð� ���Ҵ�
            createTime = Random.Range(minTime, maxTime);
        }
    }
}
