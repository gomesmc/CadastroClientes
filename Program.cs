using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;

class Program
{
    static string connectionString = "Data Source=banco.db";

    static void Main(string[] args)
    {
        CriarTabela();  
        int opcao;

        do
        {
            Console.WriteLine("\nMenu: ");
            Console.WriteLine("1 - Cadastrar cliente");
            Console.WriteLine("2 - Listar clientes");
            Console.WriteLine("3 - Sair");
            Console.Write("Escolha uma opção: ");

            string entrada = Console.ReadLine();
            bool sucesso = int.TryParse(entrada, out opcao);

            if (!sucesso)
            {
                Console.WriteLine("Opção inválida. Tente novamente.");
                continue;
            }

            switch (opcao)
            {
                case 1:
                    do
                    {
                        CadastrarCliente();

                        string resposta;
                        do
                        {
                            Console.Write("Deseja continuar cadastrando clientes? (s/n): ");
                            resposta = Console.ReadLine().ToLower();

                            if (resposta != "s" && resposta != "n")
                            {
                                Console.WriteLine("Entrada inválida. Digite apenas 's' para sim ou 'n' para não.");
                            }

                        } while (resposta != "s" && resposta != "n");

                        if (resposta != "s")
                            break;

                    } while (true);
                    break;

                case 2:
                    ListarClientes();
                    break;

                case 3:
                    Console.WriteLine("Fim do programa");
                    break;

                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }

        } while (opcao != 3);
    }

    static void CriarTabela()
    {
        using var conexao = new SQLiteConnection(connectionString);

        conexao.Execute(@"CREATE TABLE IF NOT EXISTS Pessoa (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Nome TEXT NOT NULL)");

        conexao.Execute("DELETE FROM Pessoa");
    }

    static void CadastrarCliente()
    {
        Console.Write("Digite o nome do cliente: ");
        string nome = Console.ReadLine();

        using var conexao = new SQLiteConnection(connectionString);
        conexao.Execute("INSERT INTO Pessoa (Nome) VALUES (@Nome)", new { Nome = nome });

        Console.WriteLine("Cliente cadastrado com sucesso!");
    }

    static void ListarClientes()
    {
        using var conexao = new SQLiteConnection(connectionString);
        var clientes = conexao.Query<Pessoa>("SELECT * FROM Pessoa");

        Console.WriteLine("\nClientes Cadastrados: ");

        if (clientes.AsList().Count == 0)
        {
            Console.WriteLine("Nenhum cliente cadastrado.");
        }
        else
        {
            foreach (var c in clientes)
            {
                Console.WriteLine(c.Nome);
            }
        }
    }
}
