using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    // ���� ȿ��
    Transform explosion;
    ParticleSystem expEffect;
    AudioSource expAudio;
    // ���� ����
    public float range = 5;

    // Start is called before the first frame update
    void Start()
    {
        // ������ Explosion ��ü�� ã�Ƽ� transform ��������
        explosion = GameObject.Find("Explosion").transform;
        // Explosion ��ü�� ParticleSystem ������Ʈ ������
        expEffect = explosion.GetComponent<ParticleSystem>();
        // Explosion ��ü�� AudioSource ������Ʈ ������
        expAudio = explosion.GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ���̾� ����ũ ��������
        int layerMask = 1 << LayerMask.NameToLayer("Drone");
        // ��ź�� �߽����� range ũ���� �ݰ� �ȿ� ���� ��� �˻�
        Collider[] drones = Physics.OverlapSphere(transform.position, range, layerMask);
        // ���� �ȿ� �ִ� ����� ��� ����
        foreach(Collider drone in drones)
        {
            Destroy(drone.gameObject);
        }
        // ���� ȿ���� ��ġ ����
        explosion.position = transform.position;
        // ����Ʈ ���
        expEffect.Play();
        // ����Ʈ ���� ���
        expAudio.Play();
        // ��ź ������
        Destroy(gameObject);
    }
}
