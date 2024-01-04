using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace MatrixMultiplication;

[JsonExporter]
public class MatrixMultiplication
{
    private const int N = 1500;

    private readonly double[][] _a = Generate();
    private readonly double[][] _b = Generate();

    private static double[][] Generate()
    {
        const double tmp = 1.0 / N / N;
        var a = new double[N][];

        for (var i = 0; i < N; ++i)
        {
            a[i] = new double[N];

            for (var j = 0; j < N; ++j)
                a[i][j] = tmp * (i - j) * (i + j);
        }

        return a;
    }

    private static double[][] Multiply(double[][] a, double[][] b)
    {
        var c = new double[N][];

        for (var i = 0; i < N; i++)
        {
            c[i] = new double[N];
            var ci = c[i];

            for (var k = 0; k < N; ++k)
            {
                var aik = a[i][k];
                var bk = b[k];

                for (var j = 0; j < N; ++j)
                    ci[j] += aik * bk[j];
            }
        }

        return c;
    }

    private static double[][] ParallelMultiply(double[][] a, double[][] b)
    {
        var c = new double[N][];

        Parallel.For(0, N, i => {
            c[i] = new double[N];
            var ci = c[i];

            for (var k = 0; k < N; ++k)
            {
                var aik = a[i][k];
                var bk = b[k];

                for (var j = 0; j < N; ++j)
                    ci[j] += aik * bk[j];
            }
        });

        return c;
    }

    [Benchmark]
    public double[][] Multiply() => Multiply(_a, _b);

    [Benchmark]
    public double[][] ParallelMultiply() => ParallelMultiply(_a, _b);
}

public class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<MatrixMultiplication>();
    }
}
