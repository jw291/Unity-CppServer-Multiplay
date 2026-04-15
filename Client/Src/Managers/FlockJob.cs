using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct FlockJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<float2> positions;
    [ReadOnly] public NativeArray<float2> playerPositions;
    [ReadOnly] public float balanceRadius;
    [ReadOnly] public float cohesionSeparationBalance;

    public NativeArray<float2> results;

    public void Execute(int index)
    {
        float2 myPos = positions[index];
        float2 cohesionForce = float2.zero;
        float2 separationForce = float2.zero;
        int cohesionCount = 0;
        int separationCount = 0;

        for (int i = 0; i < positions.Length; i++)
        {
            if (i == index) 
                continue;

            float distance = math.distance(myPos, positions[i]);

            if (distance < balanceRadius)
            {
                cohesionForce += positions[i];
                cohesionCount++;
            }

            if (distance < balanceRadius && distance > 0f)
            {
                separationForce += (myPos - positions[i]) / distance;
                separationCount++;
            }
        }

        cohesionForce = cohesionCount > 0
            ? math.normalize(cohesionForce / cohesionCount - myPos)
            : float2.zero;

        separationForce = separationCount > 0
            ? math.normalize(separationForce)
            : float2.zero;

        float2 balanceForce = math.lerp(cohesionForce, separationForce, cohesionSeparationBalance);
        float2 toPlayer = math.normalize(playerPositions[index] - myPos);

        if (math.length(balanceForce) < 0.1f)
            balanceForce = toPlayer;

        float2 moveDirection = balanceForce * (1f - cohesionSeparationBalance) + toPlayer * cohesionSeparationBalance;

        results[index] = math.normalizesafe(moveDirection);
    }
}
