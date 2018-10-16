struct ParticleData
{
	float2 position;
	float2 prevPosition;
	float size;
	half4 color;
};

struct EdgeData
{
	uint aID, bID;
	float length;
	float width;
	half4 color;
};