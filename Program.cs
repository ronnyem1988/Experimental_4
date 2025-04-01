// Cálculo de métricas de centralidades.
using System;
using System.Collections.Generic;

class CentralityMetrics
{
    static void Main()
    {
        // Grafo de ejemplo representado como lista de adyacencia
        var graph = new Dictionary<int, List<int>>()
        {
            {0, new List<int> {1, 2}},
            {1, new List<int> {0, 2, 3}},
            {2, new List<int> {0, 1, 4}},
            {3, new List<int> {1, 4, 5}},
            {4, new List<int> {2, 3, 5}},
            {5, new List<int> {3, 4}}
        };

        Console.WriteLine("Cálculo de Métricas de Centralidad\n");
        
        // Calcular y mostrar grado de centralidad
        var degreeCentrality = CalculateDegreeCentrality(graph);
        Console.WriteLine("Grado de Centralidad:");
        foreach (var node in degreeCentrality)
        {
            Console.WriteLine($"Nodo {node.Key}: {node.Value}");
        }

        // Calcular y mostrar cercanía de centralidad
        var closenessCentrality = CalculateClosenessCentrality(graph);
        Console.WriteLine("\nCercanía de Centralidad:");
        foreach (var node in closenessCentrality)
        {
            Console.WriteLine($"Nodo {node.Key}: {node.Value:F4}");
        }

        // Calcular y mostrar intermediación de centralidad (versión simplificada)
        var betweennessCentrality = CalculateBetweennessCentrality(graph);
        Console.WriteLine("\nIntermediación de Centralidad (simplificada):");
        foreach (var node in betweennessCentrality)
        {
            Console.WriteLine($"Nodo {node.Key}: {node.Value:F4}");
        }
    }

    // Cálculo de Grado de Centralidad
    static Dictionary<int, int> CalculateDegreeCentrality(Dictionary<int, List<int>> graph)
    {
        var degreeCentrality = new Dictionary<int, int>();
        foreach (var node in graph)
        {
            degreeCentrality[node.Key] = node.Value.Count;
        }
        return degreeCentrality;
    }

    // Cálculo de Cercanía de Centralidad
    static Dictionary<int, double> CalculateClosenessCentrality(Dictionary<int, List<int>> graph)
    {
        var closenessCentrality = new Dictionary<int, double>();
        foreach (var startNode in graph.Keys)
        {
            var distances = BFS(graph, startNode);
            double totalDistance = 0;
            foreach (var distance in distances.Values)
            {
                totalDistance += distance;
            }
            closenessCentrality[startNode] = (graph.Count - 1) / totalDistance;
        }
        return closenessCentrality;
    }

    // BFS para calcular distancias desde un nodo
    static Dictionary<int, int> BFS(Dictionary<int, List<int>> graph, int startNode)
    {
        var distances = new Dictionary<int, int>();
        var visited = new HashSet<int>();
        var queue = new Queue<int>();

        foreach (var node in graph.Keys)
        {
            distances[node] = -1;
        }

        distances[startNode] = 0;
        queue.Enqueue(startNode);
        visited.Add(startNode);

        while (queue.Count > 0)
        {
            int currentNode = queue.Dequeue();
            foreach (var neighbor in graph[currentNode])
            {
                if (!visited.Contains(neighbor))
                {
                    distances[neighbor] = distances[currentNode] + 1;
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
        return distances;
    }

    // Cálculo simplificado de Intermediación de Centralidad
    static Dictionary<int, double> CalculateBetweennessCentrality(Dictionary<int, List<int>> graph)
    {
        var betweenness = new Dictionary<int, double>();
        foreach (var node in graph.Keys)
        {
            betweenness[node] = 0;
        }

        foreach (var startNode in graph.Keys)
        {
            foreach (var endNode in graph.Keys)
            {
                if (startNode != endNode)
                {
                    var paths = GetAllShortestPaths(graph, startNode, endNode);
                    if (paths.Count > 0)
                    {
                        foreach (var node in graph.Keys)
                        {
                            if (node != startNode && node != endNode)
                            {
                                int count = 0;
                                foreach (var path in paths)
                                {
                                    if (path.Contains(node)) count++;
                                }
                                betweenness[node] += (double)count / paths.Count;
                            }
                        }
                    }
                }
            }
        }

        // Normalización (opcional)
        int n = graph.Count;
        foreach (var node in graph.Keys)
        {
            betweenness[node] = betweenness[node] / ((n - 1) * (n - 2) / 2);
        }

        return betweenness;
    }

    // Obtener todos los caminos más cortos entre dos nodos (para betweenness simplificado)
    static List<List<int>> GetAllShortestPaths(Dictionary<int, List<int>> graph, int start, int end)
    {
        var paths = new List<List<int>>();
        var queue = new Queue<List<int>>();
        queue.Enqueue(new List<int> { start });

        while (queue.Count > 0)
        {
            var path = queue.Dequeue();
            int lastNode = path[path.Count - 1];

            if (lastNode == end)
            {
                paths.Add(path);
                continue;
            }

            // Solo continuar si el camino actual es más corto o igual que los ya encontrados
            if (paths.Count == 0 || path.Count <= paths[0].Count)
            {
                foreach (var neighbor in graph[lastNode])
                {
                    if (!path.Contains(neighbor))
                    {
                        var newPath = new List<int>(path);
                        newPath.Add(neighbor);
                        queue.Enqueue(newPath);
                    }
                }
            }
        }

        // Filtrar solo los caminos más cortos
        if (paths.Count > 0)
        {
            int minLength = paths[0].Count;
            paths.RemoveAll(p => p.Count > minLength);
        }

        return paths;
    }
}