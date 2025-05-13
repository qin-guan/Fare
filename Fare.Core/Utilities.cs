using System.Linq.Expressions;

namespace Fare.Core;

public static class Utilities
{
    public static decimal GetDistance(
        List<Node> nodes,
        Expression<Func<Node, bool>> fromSelector,
        Expression<Func<Node, bool>> toSelector
    )
    {
        ArgumentNullException.ThrowIfNull(nodes);
        ArgumentNullException.ThrowIfNull(fromSelector);
        ArgumentNullException.ThrowIfNull(toSelector);

        var startNode = nodes.Single(fromSelector.Compile());
        var endNode = nodes.Single(toSelector.Compile());

        if (startNode is null)
            throw new InvalidOperationException("Could not determine the start node using the provided selector.");
        if (endNode is null)
            throw new InvalidOperationException("Could not determine the end node using the provided selector.");

        if (startNode == endNode) return 0;

        var distances = new Dictionary<Node, decimal>();
        var priorityQueue = new PriorityQueue<Node, decimal>();

        foreach (var node in nodes)
        {
            if (node == startNode)
            {
                distances[node] = 0;
                priorityQueue.Enqueue(node, 0); // Add start node to PQ with priority 0
            }
            else
            {
                distances[node] = decimal.MaxValue;
            }
        }

        while (priorityQueue.TryDequeue(out var currentNode, out var currentDistance))
        {
            if (currentNode == endNode)
            {
                return currentDistance;
            }

            if (currentDistance > distances[currentNode])
            {
                continue;
            }

            foreach (var (node, right, distance) in currentNode.Edges)
            {
                var neighborNode = node == currentNode ? right : node;
                if (!distances.ContainsKey(neighborNode))
                {
                    continue;
                }

                var distanceThroughCurrent = distances[currentNode] + distance;

                if (!(distanceThroughCurrent < distances[neighborNode])) continue;

                distances[neighborNode] = distanceThroughCurrent;
                priorityQueue.Enqueue(neighborNode, distanceThroughCurrent);
            }
        }

        return distances.GetValueOrDefault(endNode, decimal.MinValue);
    }
}