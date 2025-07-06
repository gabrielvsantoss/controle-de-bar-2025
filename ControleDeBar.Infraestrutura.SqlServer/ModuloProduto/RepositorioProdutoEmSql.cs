using ControleDeBar.Dominio.ModuloProduto;
using ControleDeBar.Dominio.ModuloProduto;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace ControleDeBar.Infraestrutura.SqlServer.ModuloProduto
{
    public class RepositorioProdutoEmSql : IRepositorioProduto
    {
        private readonly string connectionString =
        "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=controleDeBarDb;Integrated Security=True";
        public void CadastrarRegistro(Produto novoRegistro)
        {
            var sqlInserir =
             @"INSERT INTO [TBPRODUTO]
                (
                    [ID],
                    [NOME],
                    [VALOR]
                    
                )
                VALUES
                (
                    @ID,
                    @NOME,
                    @VALOR
                )";

            SqlConnection conexaoComBanco = new SqlConnection(connectionString);
            SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

            ConfigurarParametrosProduto(comandoInsercao, novoRegistro);
            conexaoComBanco.Open();

            comandoInsercao.ExecuteNonQuery();
            conexaoComBanco.Close();
        }

        public bool EditarRegistro(Guid idRegistro, Produto registroEditado)
        {
            var sqlEditar =
              @"UPDATE [TBPRODUTO]
                SET
                    [NOME] = @NOME,
                    [VALOR] = @VALOR
                WHERE
                    [ID] = @ID";
            SqlConnection conexaoComBanco = new SqlConnection(connectionString);
            SqlCommand comandoEdicao = new SqlCommand(sqlEditar, conexaoComBanco);
            
            registroEditado.Id = idRegistro;

            ConfigurarParametrosProduto(comandoEdicao, registroEditado);
            conexaoComBanco.Open();

            var linhasAfetadas = comandoEdicao.ExecuteNonQuery();
            conexaoComBanco.Close();

            return linhasAfetadas > 0;
        }

        public bool ExcluirRegistro(Guid idRegistro)
        {
            var Sqlexcluir =
             @"DELETE FROM [TBPRODUTO]
                WHERE [ID] = @ID";

            SqlConnection conexaoComBanco = new SqlConnection(connectionString);
            SqlCommand comandoExclusao = new SqlCommand(Sqlexcluir, conexaoComBanco);

            comandoExclusao.Parameters.AddWithValue("ID", idRegistro);
            conexaoComBanco.Open();

            var linhasAfetadas = comandoExclusao.ExecuteNonQuery();
            conexaoComBanco.Close();

            return linhasAfetadas > 0;
        }

        public Produto SelecionarRegistroPorId(Guid idRegistro)
        {
            var SelecionarRegistroPorId =
             @"SELECT
                    [ID],
                    [NOME], 
                    [VALOR]
                FROM 
                    [TBPRODUTO]
                WHERE 
                    [ID] = @ID";


            SqlConnection conexaoComBanco = new SqlConnection(connectionString);
            SqlCommand comandoSelecao = new SqlCommand(SelecionarRegistroPorId, conexaoComBanco);
            comandoSelecao.Parameters.AddWithValue("ID", idRegistro);
            conexaoComBanco.Open();
            SqlDataReader leitor = comandoSelecao.ExecuteReader();
            Produto? produto = null;

            if (leitor.Read())
            {
                produto = ConverterParaProduto(leitor);
            }
            return produto;
        }

        public List<Produto> SelecionarRegistros()
        {
            var sqlSelecionarTodos =
             @"SELECT
                    [ID],
                    [NOME], 
                    [VALOR]
                FROM 
                    [TBPRODUTO]";


            SqlConnection conexaoComBanco = new SqlConnection(connectionString);
            conexaoComBanco.Open();

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarTodos, conexaoComBanco);


            SqlDataReader leitor = comandoSelecao.ExecuteReader();

            var produtos = new List<Produto>();

            while (leitor.Read())
            {
                var produto = ConverterParaProduto(leitor);

                produtos.Add(produto);
            }

            conexaoComBanco.Close();
            return produtos;
        }
        private Produto ConverterParaProduto(SqlDataReader leitor)
        {
            var produto = new Produto   
                (
                    Convert.ToString(leitor["NOME"])!,
                    Convert.ToDecimal(leitor["VALOR"])!
                );

            produto.Id = Guid.Parse(leitor["ID"].ToString()!);


            return produto;
        }

        private void ConfigurarParametrosProduto(SqlCommand comando, Produto novoRegistro)
        {
            comando.Parameters.AddWithValue("ID", novoRegistro.Id);
            comando.Parameters.AddWithValue("NOME",   novoRegistro.Nome);
            comando.Parameters.AddWithValue("VALOR",   novoRegistro.Valor);
        }
    }
}
