using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AnyUI { 

    struct TriangleUVs
    {
        public Vector2 vertex1UV;
        public Vector2 vertex2UV;
        public Vector2 vertex3UV;
    };
    public class AnyUIMimeTransformUpdater : MonoBehaviour {

        [Tooltip("Should the Mime Transforms always be updated ? You can trigger an update by calling the \"ManualUpdate()\" in this script")]
        public bool UpdateEveryFrame = true;
        [Tooltip("Should every Mime Transform's up vector align to the normal vector of the surface?")]
        public bool AlignToSurfaceNormals = false;
        private ComputeShader Shader;
        private TriangleUVs[] triangleUVs;
        private AnyUiMimeTransform[] mimeTransforms;
        private Vector2[] mimeTransformUVs;
        private Vector3[] barycentricsAndTriangleIndices;
        private ComputeBuffer triangleBuffer;
        private ComputeBuffer wantedUVCoordinatesBuffer;
        private ComputeBuffer barycentricsAndTriangleIndicesOutputBuffer;
        private int[] meshIndices;
        private Vector3[] meshVertices;
        private int kernelIndex;
        public void ManualUpdate()
        {
            DispatchShaderInvocation();
            DispatchGetData();
        }

        void Start() {

            //suppress shitty assertion failed message because i use instantiate. 
            //but using it allows you to have multiple compute buffer instances of the same compute buffer
            
            Shader = (ComputeShader)Resources.Load("UVToBarycentricComputeShader");

            //fill triangle buffer for compute shader
            meshIndices = GetComponent<MeshFilter>().mesh.triangles;
            meshVertices = GetComponent<MeshFilter>().mesh.vertices;
            int meshIndex = 0;
            Vector2[] meshUVs = GetComponent<MeshFilter>().mesh.uv;
            triangleUVs = new TriangleUVs[meshIndices.Length / 3];
            for (int i = 0; i < triangleUVs.Length; i++)
            {
                triangleUVs[i].vertex1UV = meshUVs[meshIndices[meshIndex + 0]];
                triangleUVs[i].vertex2UV = meshUVs[meshIndices[meshIndex + 1]];
                triangleUVs[i].vertex3UV = meshUVs[meshIndices[meshIndex + 2]];
                meshIndex += 3;
            }
            //sizeof(TriangleUV) = 24
            triangleBuffer = new ComputeBuffer(triangleUVs.Length, 24);
            kernelIndex = Shader.FindKernel("UVToBarycentric");
            triangleBuffer.SetData(triangleUVs);
            Shader.SetBuffer(kernelIndex, "TriangleBuffer", triangleBuffer);
            //get all MimeTransforms
            mimeTransforms = GetComponentsInChildren<AnyUiMimeTransform>();
            mimeTransformUVs = new Vector2[mimeTransforms.Length];
            barycentricsAndTriangleIndices = new Vector3[mimeTransforms.Length];
            //sizeof(Vector2) = 8
            wantedUVCoordinatesBuffer = new ComputeBuffer(mimeTransformUVs.Length, 8);
            //sizeof(float3) = 12
            barycentricsAndTriangleIndicesOutputBuffer = new ComputeBuffer(mimeTransformUVs.Length, 12);
            Shader.SetBuffer(kernelIndex, "BarycentricsAndTriangleIndices", barycentricsAndTriangleIndicesOutputBuffer);
            
            Shader.SetInt("OutputBufferSize", mimeTransformUVs.Length);

        }

        private void DispatchGetData()
        {
            barycentricsAndTriangleIndicesOutputBuffer.GetData(barycentricsAndTriangleIndices);

            int triangleIndex;
            Vector3 localPosition,normal;
            int indexToVertex1, indexToVertex2, indexToVertex3;
            float barycentric1, barycentric2, barycentric3;
            for (int i = 0; i < mimeTransforms.Length; i++)
            {
                triangleIndex = Mathf.RoundToInt(barycentricsAndTriangleIndices[i].z);
                if (triangleIndex >= 0) // found a triangle which contains the tansform!
                {
                    //Debug.Log(mimeTransforms[i].name + " " + triangleIndex);
                    indexToVertex1 = meshIndices[triangleIndex * 3 + 0];
                    indexToVertex2 = meshIndices[triangleIndex * 3 + 1];
                    indexToVertex3 = meshIndices[triangleIndex * 3 + 2];
                    barycentric1 = barycentricsAndTriangleIndices[i].x;
                    barycentric2 = barycentricsAndTriangleIndices[i].y;
                    barycentric3 = 1.0f - barycentric1 - barycentric2;
                    localPosition = meshVertices[indexToVertex1] * barycentric1 +
                                    meshVertices[indexToVertex2] * barycentric2 +
                                    meshVertices[indexToVertex3] * barycentric3;
                    if (AlignToSurfaceNormals)
                    {
                        normal = Vector3.Cross(meshVertices[indexToVertex1] - meshVertices[indexToVertex2],
                                               meshVertices[indexToVertex3] - meshVertices[indexToVertex2]);
                        mimeTransforms[i].transform.up = transform.TransformDirection(normal);
                    }
                    //local position to world space point
                    mimeTransforms[i].transform.position = transform.TransformPoint(localPosition);
                    //Debug.DrawLine(transform.TransformPoint(meshVertices[indexToVertex1]), transform.TransformPoint(meshVertices[indexToVertex1]) + Camera.main.transform.position, Color.red, 0.5f);
                }
            }
        }
        private void DispatchShaderInvocation()
        {
            //update uv coordinates of transforms which have to be mimed
            for (int i = 0; i < mimeTransforms.Length; i++)
                mimeTransformUVs[i] = mimeTransforms[i].CurrentUVCoordinate;
            //invoke kernel
            wantedUVCoordinatesBuffer.SetData(mimeTransformUVs);
            Shader.SetBuffer(kernelIndex, "WantedUVs", wantedUVCoordinatesBuffer);
            //on gtx 980: more than 40 000 - 45 000 thread groups (= triangles ) && ~500 mime transforms -> bottle neck
            Shader.Dispatch(kernelIndex, triangleUVs.Length,
                                        1, 1);

        }
        private void LateUpdate()
        {
            if(UpdateEveryFrame)
                DispatchGetData();
        }


        void Update() {
            if (UpdateEveryFrame)
                DispatchShaderInvocation();
        }

        private void OnDestroy()
        {
            if(wantedUVCoordinatesBuffer != null) wantedUVCoordinatesBuffer.Release();
            if(barycentricsAndTriangleIndicesOutputBuffer != null) barycentricsAndTriangleIndicesOutputBuffer.Release();
            if (triangleBuffer != null) triangleBuffer.Release();
            triangleBuffer = wantedUVCoordinatesBuffer = barycentricsAndTriangleIndicesOutputBuffer = null;

        }
    }
}