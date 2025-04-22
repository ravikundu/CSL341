using Unity.Netcode;
using UnityEngine;

namespace Multi
{
    public class RelativeTransformSync : NetworkBehaviour
    {
        MainManager mm;
        private NetworkVariable<Vector3> relativePos = new NetworkVariable<Vector3>();
        private NetworkVariable<Quaternion> relativeRot = new NetworkVariable<Quaternion>();

        private void Start()
        {
            mm = MainManager.Instance;
        }

        void Update()
        {
            if (mm.arColab.marker == null || mm.arColab.marker.transform == null) return;

            Transform marker = mm.arColab.marker.transform;

            if (IsOwner)
            {
                relativePos.Value = marker.InverseTransformPoint(transform.position);
                relativeRot.Value = Quaternion.Inverse(marker.rotation) * transform.rotation;
            }
            else
            {
                transform.position = marker.TransformPoint(relativePos.Value);
                transform.rotation = marker.rotation * relativeRot.Value;
            }
        }
    }
}
