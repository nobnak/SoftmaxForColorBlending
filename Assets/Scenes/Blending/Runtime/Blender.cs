using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SoftmaxForColorBlending.Blending {

    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class Blender : MonoBehaviour {

        public static readonly int P_Cells = Shader.PropertyToID("_Cells");
        public static readonly int P_Cells_Length = Shader.PropertyToID("_Cells_Length");
        public static readonly int P_Clip_To_UV_Npos_Matrix = Shader.PropertyToID("_Clip_To_UV_Npos_Matrix");

        [SerializeField]
        protected Cell[] cells = new Cell[0];

        protected Camera attachedCam;
        protected ComputeBuffer cellDataBuf;
        protected List<CellDatum> cellData;

        #region unity
        private void OnEnable() {
            attachedCam = GetComponent<Camera>();
            cellData = new List<CellDatum>();
        }
        private void Update() {
            var aspect = attachedCam.aspect;
            var mvp = attachedCam.projectionMatrix * attachedCam.worldToCameraMatrix;
            var c2uvnpos = Matrix4x4.zero;
            c2uvnpos[0] = 0.5f;             c2uvnpos[12] = 0.5f;
            c2uvnpos[5] = 0.5f;             c2uvnpos[13] = 0.5f;
            c2uvnpos[2] = 0.5f * aspect;    c2uvnpos[14] = 0.5f * aspect;
            c2uvnpos[7] = 0.5f;            c2uvnpos[15] = 0.5f;

            cellData.Clear();
            for (var i = 0; i < cells.Length; i++) {
                var clip = mvp.MultiplyPoint(cells[i].tr.position);
                cellData.Add(new CellDatum() {
                    color = cells[i].color,
                    uvnpos = c2uvnpos.MultiplyPoint(clip),
                });
            }

            if (cellDataBuf != null && cellDataBuf.count != cellData.Capacity) {
                cellDataBuf?.Dispose();
                cellDataBuf = null;
            }
            if (cellData.Capacity > 0) {
                if (cellDataBuf == null)
                    cellDataBuf = new ComputeBuffer(cellData.Capacity, Marshal.SizeOf<CellDatum>());
                cellDataBuf.SetData(cellData);
            }
            Shader.SetGlobalBuffer(P_Cells, cellDataBuf);
            Shader.SetGlobalInt(P_Cells_Length, cellData.Count);
            Shader.SetGlobalMatrix(P_Clip_To_UV_Npos_Matrix, c2uvnpos);
        }
        private void OnDisable() {
            if (cellDataBuf != null) {
                cellDataBuf.Dispose();
                cellDataBuf = null;
            }
        }
        #endregion

        #region definitions
        [System.Serializable]
        public class Cell {
            public Color color;
            public Transform tr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CellDatum {
            public Color color;
            public Vector4 uvnpos;
        }
        #endregion
    }
}
