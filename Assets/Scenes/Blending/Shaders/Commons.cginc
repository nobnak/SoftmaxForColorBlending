#ifndef __BLENDER_COMMONS_INC__
#define __BLENDER_COMMONS_INC__


struct CellDatum {
	float4 color;
	float4 uvnpos;
};

StructuredBuffer<CellDatum> _Cells;
int _Cells_Length;
float4x4 _Clip_To_UV_Npos_Matrix;

#endif