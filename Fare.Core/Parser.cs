using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace Fare.Core;

public static partial class Parser
{
    [GeneratedRegex("\\|{{(.+)}} ↔ {{(.+)}}\\|\\|{{(.+)}}\\|\\|(.+)\\|\\|")]
    private static partial Regex LineMatcher();

    [GeneratedRegex(@"\|code\=.*")]
    private static partial Regex CodeRemoveMatcher();

    public static List<Node> Parse(string input)
    {
        var stns = input.Split('\n')
            .Where(line => line.StartsWith("|{{"))
            .Select(line =>
            {
                var match = LineMatcher().Match(line);
                if (!match.Success)
                {
                    throw new Exception("Failed line match");
                }

                var from = match.Groups[1].Value;
                var to = match.Groups[2].Value;
                var lineName = match.Groups[3].Value;
                var distance = match.Groups[4].Value;

                var normalizedFrom = from.Replace("Stn/", "").Replace("Layout/Station|", "");
                normalizedFrom = CodeRemoveMatcher().Replace(normalizedFrom, "");
                if (Constants.Stations.TryGetValue(normalizedFrom, out var valueFrom))
                {
                    normalizedFrom = valueFrom;
                }

                var normalizedTo = to.Replace("Stn/", "").Replace("Layout/Station|", "");
                normalizedTo = CodeRemoveMatcher().Replace(normalizedTo, "");
                if (Constants.Stations.TryGetValue(normalizedTo, out var valueTo))
                { 
                    normalizedTo = valueTo;
                }

                return new
                {
                    From = normalizedFrom,
                    To = normalizedTo,
                    LineName = lineName,
                    Distance = decimal.Parse(distance)
                };
            })
            .ToList();

        var nodes = new Dictionary<string, Node>();

        foreach (var stn in stns)
        {
            var fromNode = GetOrCreateNode(stn.From, stn.LineName);
            var toNode = GetOrCreateNode(stn.To, stn.LineName);

            toNode.Edges.Add(new Edge(toNode, fromNode, stn.Distance));
            fromNode.Edges.Add(new Edge(fromNode, toNode, stn.Distance));
        }

        return nodes.Values.ToList();

        Node GetOrCreateNode(string nodeName, string lineName)
        {
            if (nodes.TryGetValue(nodeName, out var node)) return node;
            node = new Node(nodeName, lineName, []);
            nodes.Add(nodeName, node);
            return node;
        }
    }
}