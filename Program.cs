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
            Console.WriteLine("4 - Atualizar cliente");
            Console.WriteLine("5 - Deletar cliente");
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
                    ListarClientes(true);  
                    break;

                case 3:
                    string salvar;
                    do
                    {
                        Console.Write("\nDeseja salvar a lista de clientes cadastrados? (s/n): ");
                        salvar = Console.ReadLine().ToLower();

                        if (salvar != "s" && salvar != "n")
                        {
                            Console.WriteLine("Entrada inválida. Digite apenas 's' para sim ou 'n' para não.");
                        }

                    } while (salvar != "s" && salvar != "n");

                    if (salvar == "n")
                    {
                        LimparTabela();
                        Console.WriteLine("Lista de clientes excluída");
                    }
                    else
                    {
                        Console.WriteLine("Lista de clientes salva");
                    }

                    Console.WriteLine("Fim do programa");
                    break;

                case 4:
                    AtualizarCliente();
                    break;

                case 5:
                    DeletarCliente();
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
    }

    static void LimparTabela()
    {
        using var conexao = new SQLiteConnection(connectionString);
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

    static void ListarClientes(bool mostrarId = false)
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
                Console.WriteLine(mostrarId ? $"{c.Id} - {c.Nome}" : c.Nome);
            }
        }
    }

    static void AtualizarCliente()
    {
        ListarClientes(true);

        Console.Write("\nDigite o ID do cliente que deseja atualizar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido.");
            return;
        }

        Console.Write("Digite o novo nome: ");
        string novoNome = Console.ReadLine();

        using var conexao = new SQLiteConnection(connectionString);
        int linhasAfetadas = conexao.Execute("UPDATE Pessoa SET Nome = @Nome WHERE Id = @Id", new { Nome = novoNome, Id = id });

        if (linhasAfetadas > 0)
            Console.WriteLine("Cliente atualizado com sucesso!");
        else
            Console.WriteLine("Cliente não encontrado.");
    }

    static void DeletarCliente()
    {
        ListarClientes(true);

        Console.Write("\nDigite o ID do cliente que deseja deletar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido.");
            return;
        }

        using var conexao = new SQLiteConnection(connectionString);
        int linhasAfetadas = conexao.Execute("DELETE FROM Pessoa WHERE Id = @Id", new { Id = id });

        if (linhasAfetadas > 0)
            Console.WriteLine("Cliente deletado com sucesso!");
        else
            Console.WriteLine("Cliente não encontrado.");
    }
}
