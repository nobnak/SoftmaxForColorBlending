using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace SoftmaxForColorBlending.Blending {

    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class Blender : MonoBehaviour {

        public static readonly int P_Dir = Shader.PropertyToID("_Dir");
        public static readonly int P_Colors = Shader.PropertyToID("_Colors");
        public static readonly int P_Cells = Shader.PropertyToID("_Cells");
        public static readonly int P_Cells_Length = Shader.PropertyToID("_Cells_Length");
        public static readonly int P_Clip_To_UV_Npos_Matrix = Shader.PropertyToID("_Clip_To_UV_Npos_Matrix");

        [SerializeField]
        protected Color[] colors = new Color[0];
        [SerializeField]
        protected Cell[] cells = new Cell[0];
        [SerializeField]
        protected Vector2 dir = new Vector2(1f, 0f);

        protected ComputeBuffer cellsBuf;
        protected ComputeBuffer colorBuf;
        protected List<CellDatum> cellData;

        #region unity
        private void OnEnable() {
            cellData = new List<CellDatum>();
        }
        private void Update() {
            if (colorBuf == null || colorBuf.count != colors.Length) {
                colorBuf?.Dispose();
                colorBuf = new ComputeBuffer(colors.Length, Marshal.SizeOf<Color>());
            }
            colorBuf.SetData(colors);

            cellData.Clear();
            for (var i = 0; i < cells.Length; i++) {
                cellData.Add(cells[i]);
            }

            if (cellData.Capacity > 0) {
                if (cellsBuf == null || cellsBuf.count != cellData.Count) {
                    cellsBuf?.Dispose();
                    cellsBuf = new ComputeBuffer(cellData.Capacity, Marshal.SizeOf<CellDatum>());
                }
                cellsBuf.SetData(cellData);
            }

            Shader.SetGlobalVector(P_Dir, dir.normalized);
            Shader.SetGlobalBuffer(P_Colors, colorBuf);
            Shader.SetGlobalBuffer(P_Cells, cellsBuf);
            Shader.SetGlobalInt(P_Cells_Length, cellData.Count);
        }
        private void OnDisable() {
            if (cellsBuf != null) {
                cellsBuf.Dispose();
                cellsBuf = null;
            }
            if (colorBuf != null) {
                colorBuf.Dispose();
                colorBuf = null;
            }
        }
        #endregion

        #region definitions
        [System.Serializable]
        public class Cell {
            public int icolor;
            public Vector2 uv;

            public static implicit operator CellDatum(Cell cell) {
                return new CellDatum() {
                    icolor = cell.icolor,
                    uv = cell.uv,
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CellDatum {
            public int icolor;
            public Vector2 uv;
        }
        #endregion
    }
}
