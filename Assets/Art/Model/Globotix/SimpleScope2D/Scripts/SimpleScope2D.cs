using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// SimpleScope2D v1.0

namespace Globotix
{
    public class SimpleScope2D : MonoBehaviour
    {

        public float zoomedInRadius = 1;        // 줌인된 상태의 정사영 크기
        public float zoomedOutRadius = 5;       // 줌아웃된 상태의 정사영 크기
        public float initialZoom = 2;           // 초기 줌 크기
        public float scale = 1;                 // 스코프 크기 조정 비율
        public float maxSpeed = 30f;            // 최대 이동 속도
        public float movementDampening = .2f;   // 이동 감쇄율
        public bool hideOnStart = true;         // 시작 시 스코프 숨기기 여부
        public bool holdRMBToShow = true;       // 오른쪽 마우스 버튼을 누르고 있을 때만 스코프 보이기
        public bool moveWithMouse = true;       // 마우스로 스코프 이동 여부
        public bool zoomWithWheel = true;       // 마우스 휠로 줌 조절 여부
        public bool invertWheel = false;        // 마우스 휠 줌 반전 여부
        public float zoomWheelIncrement = .25f; // 마우스 휠 줌 증가량
        public float zoomSpeed = 2f;            // 줌 속도
        public float zoomDampening = .2f;       // 줌 감쇄율
        public bool hideCursor = true;          // 스코프 보일 때 커서 숨기기 여부
        private Vector3 _scopeDestination;      // 스코프 목적지 좌표
        private Camera _scopeCamera;            // 스코프 카메라
        private GameObject _scopeUIObject;      // 스코프 UI 객체
        private float _currentZoom, _desiredZoom, _zoomVelocityRef; // 줌 관련 변수
        private Vector2 _zoomRenderSize, _scopeGraphicSize;         // 줌 렌더링 크기와 스코프 그래픽 크기
        private Vector3 _movementVelocityRef = Vector3.zero;        // 이동 속도 참조
        private AudioSource _shootSoundAudioSource;                 // 사운드 오디오 소스
        private float _screenPPU;                                   // 화면 픽셀 퍼 유닛
        private static bool _scopeVisible = true;                   // 스코프 표시 여부
        public static bool ScopeVisible { get { return _scopeVisible; } } // 스코프 표시 여부 접근자

        private void Start()
        {
            InitScope();
            if (hideOnStart)
                HideScope();
            else
                ShowScope();
        }

        void InitScope()
        {
            _shootSoundAudioSource = GetComponent<AudioSource>();

            _screenPPU = Screen.height / (Camera.main.orthographicSize * 2);
            _scopeCamera = gameObject.transform.Find("ScopeCamera").GetComponent<Camera>();
            _scopeUIObject = gameObject.transform.Find("ScopeCanvas/ScopeObject").gameObject;

            GameObject mask = gameObject.transform.Find("ScopeCanvas/ScopeObject/MaskForScope").gameObject;
            GameObject zoomedRender = gameObject.transform.Find("ScopeCanvas/ScopeObject/MaskForScope/ZoomedRender").gameObject;
            GameObject scopeGraphic = gameObject.transform.Find("ScopeCanvas/ScopeObject/ScopeImage").gameObject;
            
            // 크기 조정
            _zoomRenderSize = zoomedRender.GetComponent<RectTransform>().sizeDelta;
            _scopeGraphicSize = scopeGraphic.GetComponent<RectTransform>().sizeDelta;
            zoomedRender.GetComponent<RectTransform>().sizeDelta = _zoomRenderSize * scale;
            mask.GetComponent<RectTransform>().sizeDelta = _zoomRenderSize * scale;
            scopeGraphic.GetComponent<RectTransform>().sizeDelta = _scopeGraphicSize * scale;
            
            Zoom = initialZoom;
            MoveScopeToWorldCoordinates(_scopeCamera.transform.position);
        }

        void Update()
        {
            if (!_scopeCamera) return;

            if (moveWithMouse)
            {
                _scopeDestination = Input.mousePosition;
            }

            // 스코프 이동
            Vector3 scopePosition = Vector3.SmoothDamp(_scopeUIObject.transform.position, _scopeDestination, ref _movementVelocityRef, movementDampening, maxSpeed * _screenPPU);
            PositionScope(scopePosition);

            if (zoomWithWheel)
            {
                float scrollChange = Input.GetAxis("Mouse ScrollWheel");
                if (!invertWheel) scrollChange = -scrollChange;
                scrollChange = normalizeFloat(scrollChange);
                _desiredZoom += scrollChange * zoomWheelIncrement;
                ZoomTo(_desiredZoom); // 줌 범위 내로 조정
            }

            // 줌 조정
            if (_desiredZoom != _currentZoom)
            {
                _currentZoom = Mathf.SmoothDamp(_currentZoom, _desiredZoom, ref _zoomVelocityRef, zoomDampening);
                showCurrentZoom();
            }
            if (holdRMBToShow)
            {
                //if (Input.GetMouseButtonDown(1))
                //{
                    //MoveScopeTo(Input.mousePosition);
                    //ShowScope();
                //}
                //if (Input.GetMouseButtonUp(1))
                //{
                    //HideScope();
                //}
            }

            // 클릭 시 오디오 재생
            if (Input.GetMouseButtonDown(0))
            {
                if (_shootSoundAudioSource != null)
                {
                    if (_scopeVisible)
                    {
                        _shootSoundAudioSource.Play();
                    }
                }
            }
        }

        public void ShowScope()
        {
            _scopeUIObject.SetActive(true);
            _scopeVisible = true;
            if (hideCursor) Cursor.visible = false;
        }

        public void HideScope()
        {
            _scopeUIObject.SetActive(false);
            _scopeVisible = false;
            if (hideCursor) Cursor.visible = true;
        }

        public void SetScopeDestination(Vector2 screenCoordinates)
        {
            // 스코프가 속도와 감쇄율을 기준으로 목적지로 이동함
            _scopeDestination = screenCoordinates;
        }

        public void SetScopeDestinationWorldUnits(Vector2 worldCoordinates)
        {
            // 스코프가 속도와 감쇄율을 기준으로 목적지로 이동함
            _scopeDestination = Camera.main.WorldToScreenPoint(worldCoordinates);
        }

        public void MoveScopeTo(Vector2 screenCoordinates)
        {
            // 스코프를 즉시 위치로 이동함
            _scopeDestination = screenCoordinates;
            PositionScope(screenCoordinates);
        }

        public void MoveScopeToWorldCoordinates(Vector2 worldCoordinates)
        {
            // 스코프를 즉시 위치로 이동함
            _scopeDestination = Camera.main.WorldToScreenPoint(worldCoordinates);
            PositionScope(_scopeDestination);
        }

        public void ZoomTo(float desiredZoom)
        {
            // 스코프가 줌 속도와 감쇄율을 기준으로 줌 조절함
            _desiredZoom = Mathf.Clamp(desiredZoom, zoomedInRadius, zoomedOutRadius);
        }

        public float Zoom
        {
            // 즉시 줌 레벨 설정
            get
            {
                return _currentZoom;
            }
            set
            {
                _currentZoom = Mathf.Clamp(value, zoomedInRadius, zoomedOutRadius);
                _desiredZoom = _currentZoom;
                showCurrentZoom();
            }
        }

        public void SetScale(float myScale)
        {
            scale = myScale;
            GameObject mask = gameObject.transform.Find("ScopeCanvas/ScopeObject/MaskForScope").gameObject;
            GameObject zoomedRender = gameObject.transform.Find("ScopeCanvas/ScopeObject/MaskForScope/ZoomedRender").gameObject;
            GameObject scopeGraphic = gameObject.transform.Find("ScopeCanvas/ScopeObject/ScopeImage").gameObject;

            zoomedRender.GetComponent<RectTransform>().sizeDelta = _zoomRenderSize * scale;
            mask.GetComponent<RectTransform>().sizeDelta = _zoomRenderSize * scale;
            scopeGraphic.GetComponent<RectTransform>().sizeDelta = _scopeGraphicSize * scale;
        }

        private void showCurrentZoom()
        {
            // 내부 용도로만 사용
            _currentZoom = Mathf.Clamp(_currentZoom, zoomedInRadius, zoomedOutRadius);

            _scopeCamera.orthographicSize = _currentZoom;
        }

        private void OnValidate()
        {
            if (Application.isPlaying && Application.isEditor)
            {
                // SetScale(scale);
            }
        }

        private void PositionScope(Vector2 screenCoordinates)
        {
            Vector3 newScopePosition = screenCoordinates;
            // 깊이 값을 추가하여 월드 포인트로 변환
            newScopePosition.z = 10;
            Vector3 cameraDest = Camera.main.ScreenToWorldPoint(newScopePosition);
            // 카메라의 z 값을 유지함
            cameraDest.z = _scopeCamera.transform.position.z;
            _scopeCamera.transform.position = cameraDest;
            _scopeUIObject.transform.position = newScopePosition;
        }

        private void PositionScopeWorldCoordinates(Vector3 worldCoordinates)
        {
            Vector3 cameraDest = worldCoordinates;
            // 카메라의 z 값을 유지함
            cameraDest.z = _scopeCamera.transform.position.z;
            _scopeCamera.transform.position = cameraDest;
            Vector3 newScopePosition = Camera.main.WorldToScreenPoint(cameraDest);
            _scopeUIObject.transform.position = newScopePosition;
        }

        float normalizeFloat(float num)
        {
            if (num > 0) num = 1;
            if (num < 0) num = -1;
            return num;
        }
    }
}
