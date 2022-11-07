using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GrabObject : MonoBehaviour
    {
        // �ʿ� �Ӽ� : ��ü�� ��� �ִ��� ���� , ����ִ� ��ü, ���� ��ü�� ����, ���� �� �ִ� �Ÿ�
        // ��ü�� ��� �ִ����� ����
        bool isGrabbing = false;
        // ��� �ִ� ��ü
        GameObject grabbedObject;
        // ���� ��ü�� ����
        public LayerMask grabbedLayer;
        // ���� �� �ִ� �Ÿ�
        public float grabRange = 0.2f;
        // ���� ��ġ
        Vector3 prePos;
        // ���� ��
        float throwPower = 10;

        // ���� ȸ��
        Quaternion preRot;
        // ȸ����
        public float rotPower = 5;

        // ���Ÿ����� ��ü�� ��� ��� Ȱ��ȭ ����
        public bool isRemoteGrab = true;
        // ���Ÿ����� ��ü�� ���� �� �ִ� �Ÿ�
        public float remoteGrabDistance = 20;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // ��ü ���
            // 1. ��ü�� ���� �ʰ� ���� ���
            if (!isGrabbing)
            {
                // ��� �뵵
                TryGrab();
            }
            else
            {
                // ��ü ����
                TryUngrab();
            }
        }
        private void TryGrab()
        {
            // [Grab]��ư�� ������ ���� ���� �ȿ� �ִ� ��ź�� ��´�
            // 1. [Grab] ��ư�� �����ٸ�
            if (ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
            {
                // ���Ÿ� ��ü ��⸦ ����ߴٸ�
                if (isRemoteGrab)
                {
                    // �� �������� Ray����
                    Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
                    RaycastHit hitInfo;
                    // SphereCast�� �̿��� ��ü �浹�� üũ
                    if (Physics.SphereCast(ray, 0.5f, out hitInfo, remoteGrabDistance, grabbedLayer))
                    {
                        // ���� ���·� ��ȯ
                        isGrabbing = true;
                        // ���� ��ü�� ���� ���
                        grabbedObject = hitInfo.transform.gameObject;
                        // ��ü�� �������±�� ����
                        StartCoroutine(GrabbingAnimation());
                    }
                }
                // 2. ���� ���� �ȿ� ��ź�� ���� ��
                // ���� �ȿ� �ִ� ��� ��ź ����
                Collider[] hitobjects = Physics.OverlapSphere(ARAVRInput.RHandPosition, grabRange, grabbedLayer);

                // ���� ����� ��ź �ε���
                int closest = 0;
                for (int i = 1; i < hitobjects.Length; i++)
                {
                    // �հ� ���� ����� ��ü���ǰŸ�
                    Vector3 closetPos = hitobjects[closest].transform.position;
                    float closestDistance = Vector3.Distance(closetPos, ARAVRInput.RHandPosition);
                    // ���� ��ü�� ���� �Ÿ�
                    Vector3 nextPos = hitobjects[i].transform.position;
                    float nextDistance = Vector3.Distance(nextPos, ARAVRInput.RHandPosition);
                    // ���� ��ü���� �Ÿ��� �� �����ٸ�
                    if (nextDistance < closestDistance)
                    {
                        // ���� ����� ��ü �ε��� ��ü
                        closest = i;
                    }
                }
                // 3. ��ź�� ��´�
                // ����� ��ü�� ���� ���
                if (hitobjects.Length > 0)
                {
                    // ���� ���·� ��ȯ
                    isGrabbing = true;
                    // ���� ��ü�� ���� ���
                    grabbedObject = hitobjects[closest].gameObject;
                    // ���� ��ü�� ���� �ڽ����� ���
                    grabbedObject.transform.parent = ARAVRInput.RHand;

                    // ���� ��� ����
                    grabbedObject.GetComponent<Rigidbody>().isKinematic = true;

                    // �ʱ� ��ġ �� ����
                    prePos = ARAVRInput.RHandPosition;

                    // �ʱ� ȸ���� ����
                    preRot = ARAVRInput.RHand.rotation;
                }

            }
        }
        private void TryUngrab()
        {
            // ���� ����
            Vector3 throwDirection = ARAVRInput.RHandPosition - prePos;
            // ��ġ ���
            prePos = ARAVRInput.RHandPosition;
            // ���ʹϾ� ����
            // angle1 = Q1, angle2 = Q2
            // angle1 + angle2 = Q1 * Q2
            // -angle2 = Quaternion.Inverse(Q2)
            // angle2 - angle1 = Quaternion.FromToRotation(Q1,Q2) = Q2 * Quaternion.Inverse(Q1)
            // ȸ�� ���� = current - previous�� �� �� ����. -previous�� Inverse�� ����
            Quaternion deltaRotation = ARAVRInput.RHand.rotation * Quaternion.Inverse(preRot);
            // ���� ȸ�� ����
            preRot = ARAVRInput.RHand.rotation;

            // ��ư�� ���Ҵٸ�
            if (ARAVRInput.GetUp(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
            {
                // ���� ���� ���·� ��ȭ
                isGrabbing = false;
                // ���� ��� Ȱ��ȭ
                grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
                // �տ��� ��ź �����
                grabbedObject.transform.parent = null;

                // ������
                grabbedObject.GetComponent<Rigidbody>().velocity = throwDirection * throwPower;

                // ���ӵ� = (1/dt) * d??��(Ư�� �� ���� ���� ����)
                float angle;
                Vector3 axis;
                deltaRotation.ToAngleAxis(out angle, out axis);
                Vector3 angularVelocity = 1.0f / Time.deltaTime * angle * axis;
                grabbedObject.GetComponent<Rigidbody>().angularVelocity = angularVelocity;


                // ���� ��ü�� ������ ����
                grabbedObject = null;
            }
        }

        IEnumerator GrabbingAnimation()
        {
            // ������� ����
            grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
            // �ʱ� ��ġ �� ����
            prePos = ARAVRInput.RHandPosition;
            // �ʱ� ȸ�� �� ����
            preRot = ARAVRInput.RHand.rotation;
            Vector3 startLocation = grabbedObject.transform.position;
            Vector3 targetLocation = ARAVRInput.RHandPosition + ARAVRInput.RHandDirection * 0.1f;

            float currnetTime = 0;
            float finishTime = 0.2f;
            // �����
            float elapsedRate = currnetTime / finishTime;
            while (elapsedRate < 1)
            {
                currnetTime += Time.deltaTime;
                elapsedRate = currnetTime / finishTime;
                grabbedObject.transform.position = Vector3.Lerp(startLocation, targetLocation, elapsedRate);
                yield return null;
            }
            // ���� ��ü�� ���� �ڽ����� ���
            grabbedObject.transform.position = targetLocation;
            grabbedObject.transform.parent = ARAVRInput.RHand;
        }
    }
}