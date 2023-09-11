namespace Curve_Camera_Controller
{
    using UnityEngine;

    public class CurveCameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;

        [Space(10), SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

        [Space(10), SerializeField, Range(1.0f, 20.0f)] private float minDistance = 2.0f;
        [SerializeField, Range(1.0f, 20.0f)] private float maxDistance = 12.0f;

        [Space(10), SerializeField, Range(1.0f, 10.0f)] private float height = 1.8f;

        [Space(10), SerializeField, Range(1.0f, 20.0f)] private float yRotationDamping = 10.0f;
        [SerializeField, Range(1.0f, 20.0f)] private float xRotationDamping = 16.0f;

        [Space(10), SerializeField, Range(0.0f, 180f)] private float maxVerticalAngle = 60.0f;
        [SerializeField, Range(-180f, 0.0f)] private float minVerticalAngle = -30.0f;

        private Transform _transform;


        private bool _gamePaused;

        public void SetPauseState(bool state)
        {
            _gamePaused = state;
        }


        private void Awake()
        {
            _transform = transform;
        }


        private void LateUpdate()
        {
            if (_gamePaused)
                return;

            var currentRotation = _transform.eulerAngles;
            var currentHorizontalAngle = currentRotation.y;
            var currentVerticalAngle = currentRotation.x;

            var desiredHorizontalAngle = currentHorizontalAngle + Input.GetAxis("Mouse X") * xRotationDamping;
            var desiredVerticalAngle = currentVerticalAngle - Input.GetAxis("Mouse Y") * yRotationDamping;

            desiredVerticalAngle = desiredVerticalAngle >= 180f ? desiredVerticalAngle - 360f : desiredVerticalAngle;
            desiredVerticalAngle = Mathf.Clamp(desiredVerticalAngle, minVerticalAngle, maxVerticalAngle);

            currentHorizontalAngle = Mathf.LerpAngle(currentHorizontalAngle, desiredHorizontalAngle, yRotationDamping * Time.deltaTime);
            currentVerticalAngle = Mathf.LerpAngle(currentVerticalAngle, desiredVerticalAngle, yRotationDamping * Time.deltaTime);

            _transform.eulerAngles = new Vector3(currentVerticalAngle, currentHorizontalAngle, 0f);

            var checkAngle = currentVerticalAngle >= 180f ? currentVerticalAngle - 360f : currentVerticalAngle;
            var difference = (checkAngle - minVerticalAngle) / (maxVerticalAngle - minVerticalAngle);
            var curveDistance = Mathf.Lerp(minDistance, maxDistance, curve.Evaluate(difference));

            var desiredPosition = target.position - _transform.forward * curveDistance + Vector3.up * height;

            _transform.position = desiredPosition;
        }
    }
}