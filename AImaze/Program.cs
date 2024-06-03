using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

class MazeAI
{
    static int width = new Random().Next(5, 20);
    static int height = new Random().Next(5, 20);
    static int[,] maze = new int[width, height];
    static List<List<(int, int)>> solutions = new List<List<(int, int)>>();
    static List<(int, int)> shortestPath;
    static List<(int, int)> longestPath;

    static void Main(string[] args)
    {
        GenerateMaze();
        PrintMaze();

        Console.WriteLine("Gathering solutions...");

        List<(int, int)> path = new List<(int, int)>();
        bool[,] visited = new bool[width, height];
        FindSolutions(0, 0, path, visited);

        Console.WriteLine($"Number of solutions: {solutions.Count}");
        Console.WriteLine("Solutions:");
        foreach (var solution in solutions)
        {
            Console.WriteLine(string.Join(" -> ", solution));
        }

        Console.WriteLine("\n Algorithm Test:");
        Test();

        Console.ReadLine();
    }

    static void GenerateMaze()
    {
        maze = new int[width, height];
        Stack<(int, int)> stack = new Stack<(int, int)>();
        Random rand = new Random();
        int x = 0, y = 0;
        maze[x, y] = 1;
        stack.Push((x, y));

        while (stack.Count > 0)
        {
            var cell = stack.Pop();
            x = cell.Item1;
            y = cell.Item2;
            List<(int, int)> neighbors = new List<(int, int)>();

            if (x > 1 && maze[x - 2, y] == 0) neighbors.Add((x - 2, y));
            if (x < width - 2 && maze[x + 2, y] == 0) neighbors.Add((x + 2, y));
            if (y > 1 && maze[x, y - 2] == 0) neighbors.Add((x, y - 2));
            if (y < height - 2 && maze[x, y + 2] == 0) neighbors.Add((x, y + 2));

            if (neighbors.Count > 0)
            {
                stack.Push(cell);
                var next = neighbors[rand.Next(neighbors.Count)];
                int nx = next.Item1;
                int ny = next.Item2;
                maze[nx, ny] = 1;
                maze[(x + nx) / 2, (y + ny) / 2] = 1;
                stack.Push(next);
            }
        }

        CreatePathToEnd();
    }

    static void CreatePathToEnd()
    {
        for (int i = 0; i < width; i++)
        {
            maze[i, 0] = 1;
            maze[i, height - 1] = 1;
        }
        for (int j = 0; j < height; j++)
        {
            maze[0, j] = 1;
            maze[width - 1, j] = 1;
        }
    }

    static void PrintMaze()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(new string('=', width * 2));
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                sb.Append(maze[x, y] == 1 ? "  " : "##");
            }
            sb.AppendLine();
        }
        sb.AppendLine(new string('=', width * 2));
        Console.WriteLine(sb.ToString());
    }

    static void PrintSolution(List<(int, int)> solution)
    {
        int[,] solutionMaze = (int[,])maze.Clone();
        foreach (var cell in solution)
        {
            solutionMaze[cell.Item1, cell.Item2] = 2;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(new string('=', width * 2));
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (solutionMaze[x, y] == 2)
                    sb.Append("XX");
                else
                    sb.Append(solutionMaze[x, y] == 1 ? "  " : "##");
            }
            sb.AppendLine();
        }
        sb.AppendLine(new string('=', width * 2));
        Console.WriteLine(sb.ToString());
    }

    static void FindSolutions(int x, int y, List<(int, int)> path, bool[,] visited)
    {
        if (x < 0 || y < 0 || x >= width || y >= height || maze[x, y] == 0 || visited[x, y])
        {
            return;
        }

        path.Add((x, y));
        visited[x, y] = true;

        if (x == width - 1 && y == height - 1)
        {
            solutions.Add(new List<(int, int)>(path));
        }
        else
        {
            FindSolutions(x + 1, y, path, visited);
            FindSolutions(x - 1, y, path, visited);
            FindSolutions(x, y + 1, path, visited);
            FindSolutions(x, y - 1, path, visited);
        }

        path.RemoveAt(path.Count - 1);
        visited[x, y] = false;
    }

    static void Test()
    {
        Stopwatch stopwatch = new Stopwatch();

        stopwatch.Start();
        List<(int, int)> dfsPath = new List<(int, int)>();
        bool[,] visitedDFS = new bool[width, height];
        bool foundDFS = DFS(0, 0, dfsPath, visitedDFS);
        stopwatch.Stop();
        if (foundDFS)
        {
            Console.WriteLine($"DFS Agent found a solution in {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
            if (shortestPath == null || dfsPath.Count < shortestPath.Count)
            {
                shortestPath = dfsPath;
            }
            if (longestPath == null || dfsPath.Count > longestPath.Count)
            {
                longestPath = dfsPath;
            }
        }
        else
        {
            Console.WriteLine("DFS Null");
        }

        stopwatch.Restart();
        List<(int, int)> bfsPath = BFS();
        stopwatch.Stop();
        if (bfsPath.Count > 0)
        {
            Console.WriteLine($"BFS Agent found a solution in {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
            if (shortestPath == null || bfsPath.Count < shortestPath.Count)
            {
                shortestPath = bfsPath;
            }
            if (longestPath == null || bfsPath.Count > longestPath.Count)
            {
                longestPath = bfsPath;
            }
        }
        else
        {
            Console.WriteLine("BFS Null");
        }

        if (shortestPath != null)
        {
            Console.WriteLine("Shortest solution:");
            PrintSolution(shortestPath);
        }

        if (longestPath != null)
        {
            Console.WriteLine("Longest solution:");
            PrintSolution(longestPath);
        }
    }

    static bool DFS(int x, int y, List<(int, int)> path, bool[,] visited)
    {
        if (x < 0 || y < 0 || x >= width || y >= height || maze[x, y] == 0 || visited[x, y])
        {
            return false;
        }

        path.Add((x, y));
        visited[x, y] = true;

        if (x == width - 1 && y == height - 1)
        {
            return true;
        }

        if (DFS(x + 1, y, path, visited) ||
            DFS(x - 1, y, path, visited) ||
            DFS(x, y + 1, path, visited) ||
            DFS(x, y - 1, path, visited))
        {
            return true;
        }

        path.RemoveAt(path.Count - 1);
        visited[x, y] = false;
        return false;
    }

    static List<(int, int)> BFS()
    {
        Queue<List<(int, int)>> queue = new Queue<List<(int, int)>>();
        bool[,] visited = new bool[width, height];
        List<(int, int)> startPath = new List<(int, int)> { (0, 0) };
        queue.Enqueue(startPath);
        visited[0, 0] = true;

        while (queue.Count > 0)
        {
            var path = queue.Dequeue();
            var cell = path[path.Count - 1];
            int x = cell.Item1;
            int y = cell.Item2;

            if (x == width - 1 && y == height - 1)
            {
                return path;
            }

            List<(int, int)> neighbors = new List<(int, int)>
            {
                (x + 1, y), (x - 1, y), (x, y + 1), (x, y - 1)
            };

            foreach (var neighbor in neighbors)
            {
                int nx = neighbor.Item1;
                int ny = neighbor.Item2;
                if (nx >= 0 && ny >= 0 && nx < width && ny < height && maze[nx, ny] == 1 && !visited[nx, ny])
                {
                    visited[nx, ny] = true;
                    var newPath = new List<(int, int)>(path) { (nx, ny) };
                    queue.Enqueue(newPath);
                }
            }
        }

        return new List<(int, int)>();
    }
}
