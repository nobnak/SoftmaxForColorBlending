#ifndef __BLENDER_COMMONS_INC__
#define __BLENDER_COMMONS_INC__


struct Cell {
	int icolor;
	float2 uv;
};

StructuredBuffer<float4> _Colors;
StructuredBuffer<Cell> _Cells;
int _Cells_Length;

#endif