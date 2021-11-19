using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Candlelight;


namespace MaxGame {

    public class MapController : MonoBehaviour, IPointerClickHandler, IScrollHandler, IPointerExitHandler, IPointerEnterHandler {

        [SerializeField, PropertyBackingField(nameof(CameraObject))]
        private GameObject cameraObject;
        public GameObject CameraObject { get => cameraObject; set => cameraObject = value; }

        protected Camera MapCamera;

        protected OverheadCamera OverheadCamera;

        protected GameController GameController;

        protected Vector3 ReturnPosition;

        protected Material Material;

        protected GameObject NormalButton;

        protected GameObject OutlineButton;

        protected Camera MinimapCamera;

        void Awake() {

            
            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();
            
            MinimapCamera = GameObject.Find("/GameController/MinimapCam").GetComponent<Camera>();

            MapCamera = CameraObject.GetComponent<Camera>();
            ReturnPosition = MapCamera.transform.position;

            OutlineButton = transform.Find("OutlineButton").gameObject;
            NormalButton = transform.Find("NormalButton").gameObject;

            Button tempOutlineButton = OutlineButton.GetComponent<Button>();

            tempOutlineButton.onClick.AddListener(() => OutlineMode());

            Button tempNormalButton = NormalButton.GetComponent<Button>();
            tempNormalButton.onClick.AddListener(() => NormalMode());


            Material = GetComponent<RawImage>().material;

            Material.DisableKeyword("OUTLINE_MODE");
            Material.EnableKeyword("NORMAL_MODE");

            OutlineButton.SetActive(true);
            NormalButton.SetActive(false);

        }

        void Start() {

            OverheadCamera = GameController.GetOverhead();
            
        }

        public void OnScroll(PointerEventData eventData) {

            Zoom(eventData.position, eventData.scrollDelta.y);

        }

        public void OnPointerEnter(PointerEventData eventData) {

            OverheadCamera.Freeze();

        }

        public void OnPointerExit(PointerEventData eventData) {

            OverheadCamera.Unfreeze();
        }

        public void OnSelect(PointerEventData eventData) {

        }

        protected void Zoom(Vector2 uvCoords, float zoomScalar) {


            RectTransform rectTransform = GetComponent<RectTransform>();
            Rect rect = rectTransform.rect;

            Vector2 zoomPoint; 
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, Camera.main, out zoomPoint);

            int px = Mathf.Clamp (0,(int)(((zoomPoint.x-rect.x)*500)/rect.width),500);
            int py = Mathf.Clamp (0,(int)(((zoomPoint.y-rect.y)*500)/rect.height),500);

            Vector3 worldPoint = new Vector3(px * 4, 0f, py * 4);

            RaycastHit hit;

            Ray rayCast = new Ray(MapCamera.transform.position, MapCamera.transform.forward); // new Vector3(uvCoords.x * 1000, 400f, uvCoords.y * 1000));


            Physics.Raycast(rayCast, out hit, 1000);
            

            if (hit.collider != null) {
                    
                if ((zoomScalar > 0) && (MapCamera.transform.position.y > 300)) {
                    MapCamera.transform.position = Vector3.MoveTowards(MapCamera.transform.position, worldPoint /*rayCast.GetPoint(500f)*/, 2000f * Time.deltaTime);
                }
                else if ((zoomScalar < 0) && (MapCamera.transform.position.y < 980)) {
                    MapCamera.transform.position = Vector3.MoveTowards(MapCamera.transform.position, ReturnPosition, 2000f * Time.deltaTime);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData) {

            Debug.Log("Button click detected in map controller");
        }

        public void OutlineMode() {

            Material.EnableKeyword("OUTLINE_MODE");
            Material.DisableKeyword("NORMAL_MODE");

            int layerMask = 12042;
            MinimapCamera.cullingMask = layerMask;
            
            OutlineButton.SetActive(false);
            NormalButton.SetActive(true);
        }

        public void NormalMode() {

            Material.DisableKeyword("OUTLINE_MODE");
            Material.EnableKeyword("NORMAL_MODE");

            int layerMask = 12058;

            MinimapCamera.cullingMask = layerMask;

            OutlineButton.SetActive(true);
            NormalButton.SetActive(false);
        }
    }   
}
