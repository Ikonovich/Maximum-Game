using System;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;


namespace MaxGame {


    public class ScreenSpaceEffect : MonoBehaviour {

        [SerializeField, PropertyBackingField(nameof(Material))]
        private Material material;
        public Material Material { get => material; set => material = value; }
        

        [SerializeField, PropertyBackingField(nameof(Shader))]
        private Shader shader;
        public Shader Shader { get => shader; set => shader = value; }

        [SerializeField, PropertyBackingField(nameof(Light))]
        private GameObject light;
        public GameObject Light { get => light; set => light = value; }
        
        
        protected Camera Camera;

        protected Transform SunLight;

        void Awake() {

            SunLight = Light.transform;

            Camera = GetComponent<Camera>();

            Material = new Material(Shader);
        }

        //[ImageEffectOpaque]
        void OnRenderImage(RenderTexture source, RenderTexture destination) {

            if (!Material) {

                Graphics.Blit(source, destination);
                return;
            }
            else {  

                // Constructing a model matrix for a torus
                Matrix4x4 Torus = Matrix4x4.TRS(Vector3.right, 
                    Quaternion.identity, Vector3.one);
                Torus *= Matrix4x4.TRS(Vector3.zero, 
                    Quaternion.Euler(new Vector3(Time.time % 360 * 50, 0, 0)), 
                    Vector3.one);

                Material.SetMatrix("_MatTorus_InvModel", Torus.inverse);
                Material.SetMatrix("_FrustrumCornersES", GetFrustumCorners(Camera));
                Material.SetMatrix("_CameraInvViewMatrix", Camera.cameraToWorldMatrix);
                Material.SetVector("_CameraWS", Camera.transform.position);
                Material.SetVector("_LightDir", SunLight ? SunLight.forward : Vector3.down);


                CustomGraphicsBlit(source, destination, Material, 0);
            }
        }

        private Matrix4x4 GetFrustumCorners(Camera camera) {

            float camFov = camera.fieldOfView;
            float camAspect = camera.aspect;

            Matrix4x4 frustumCorners = Matrix4x4.identity;

            float fovHalf = camFov * 0.5f;
            float tan_fov = Mathf.Tan(fovHalf * Mathf.Deg2Rad);

            Vector3 toRight = Vector3.right * tan_fov * camAspect;
            Vector3 toTop = Vector3.up * tan_fov;

            Vector3 topLeft = (-Vector3.forward - toRight + toTop);
            Vector3 topRight = (-Vector3.forward + toRight + toTop);
            Vector3 bottomRight = (-Vector3.forward + toRight - toTop);
            Vector3 bottomLeft = (-Vector3.forward - toRight - toTop);

            frustumCorners.SetRow(0, topLeft);
            frustumCorners.SetRow(1, topRight);
            frustumCorners.SetRow(2, bottomRight);
            frustumCorners.SetRow(3, bottomLeft);

            return frustumCorners;

        }

        static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material materialIn, int passNum) {

            RenderTexture.active = dest;

            materialIn.SetTexture("_MainTex", source);

            GL.PushMatrix();
            GL.LoadOrtho();

            materialIn.SetPass(passNum);

            GL.Begin(GL.QUADS);

            GL.MultiTexCoord2(0, 0.0f, 0.0f);
            GL.Vertex3(0.0f, 0.0f, 3.0f);

            GL.MultiTexCoord2(0, 1.0f, 0.0f);
            GL.Vertex3(1.0f, 0.0f, 2.0f);

            GL.MultiTexCoord2(0, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);

            GL.MultiTexCoord2(0, 0.0f, 1.0f);
            GL.Vertex3(0.0f, 1.0f, 0.0f);

            GL.End();
            GL.PopMatrix();
        }
    }
}