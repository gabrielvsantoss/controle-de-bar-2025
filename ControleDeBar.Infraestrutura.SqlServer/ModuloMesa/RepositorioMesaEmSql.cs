using ControleDeBar.Dominio.ModuloMesa;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace ControleDeBar.Infraestrutura.SqlServer.ModuloMesa
{
    public class RepositorioMesaEmSql : IRepositorioMesa
    {
        private readonly string connectionString =
        "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=controleDeBarDb;Integrated Security=True";
        public void CadastrarRegistro(Mesa novoRegistro)
        {
            var sqlInserir =
             @"INSERT INTO [TBMESA]
                (
                    [ID],
                    [NUMERO],
                    [CAPACIDADE],
                    [ESTAOCUPADA]
                )
                VALUES
                (
                    @ID,
                    @NUMERO,
                    @CAPACIDADE,
                    @ESTAOCUPADA
                )";

            SqlConnection conexaoComBanco = new SqlConnection(connectionString);
            SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

            ConfigurarParametrosMesa(comandoInsercao, novoRegistro);
            conexaoComBanco.Open();

            comandoInsercao.ExecuteNonQuery();
            conexaoComBanco.Close();
        }

        public bool EditarRegistro(Guid idRegistro, Mesa registroEditado)
        {
            var sqlEditar =
              @"UPDATE [TBMESA]
                SET
                    [NUMERO] = @NUMERO,
                    [CAPACIDADE] = @CAPACIDADE,
                    [ESTAOCUPADA] = @ESTAOCUPADA
                WHERE
                    [ID] = @ID";
            SqlConnection conexaoComBanco = new SqlConnection(connectionString);
            SqlCommand comandoEdicao = new SqlCommand(sqlEditar, conexaoComBanco);

            registroEditado.Id = idRegistro;

            ConfigurarParametrosMesa(comandoEdicao, registroEditado);
            conexaoComBanco.Open();

            var linhasAfetadas = comandoEdicao.ExecuteNonQuery();
            conexaoComBanco.Close();

            return linhasAfetadas > 0;
        }

        public bool ExcluirRegistro(Guid idRegistro)
        {
            var Sqlexcluir =
             @"DELETE FROM [TBMESA]
                WHERE [ID] = @ID";

            SqlConnection conexaoComBanco = new SqlConnection(connectionString);
            SqlCommand comandoExclusao = new SqlCommand(Sqlexcluir, conexaoComBanco);

            comandoExclusao.Parameters.AddWithValue("ID", idRegistro);
            conexaoComBanco.Open();

            var linhasAfetadas = comandoExclusao.ExecuteNonQuery();
            conexaoComBanco.Close();

            return linhasAfetadas > 0;
        }

        public Mesa SelecionarRegistroPorId(Guid idRegistro)
        {
            var SelecionarRegistroPorId =
             @"SELECT
                    [ID],
                    [NUMERO], 
                    [CAPACIDADE],
                    [ESTAOCUPADA]
                FROM 
                    [TBMESA]
                WHERE 
                    [ID] = @ID";


            SqlConnection conexaoComBanco = new SqlConnection(connectionString);
            SqlCommand comandoSelecao = new SqlCommand(SelecionarRegistroPorId, conexaoComBanco);
            comandoSelecao.Parameters.AddWithValue("ID", idRegistro);
            conexaoComBanco.Open();
            SqlDataReader leitor = comandoSelecao.ExecuteReader();
            Mesa? mesa = null;

            if (leitor.Read())
            {
                mesa = ConverterParaMesa(leitor);
            }
            return mesa;
        }

        public List<Mesa> SelecionarRegistros()
        {
            var sqlSelecionarTodos =
             @"SELECT
                    [ID],
                    [NUMERO], 
                    [CAPACIDADE],
                    [ESTAOCUPADA]
                FROM 
                    [TBMESA]";


            SqlConnection conexaoComBanco = new SqlConnection(connectionString);
            conexaoComBanco.Open();

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarTodos, conexaoComBanco);


            SqlDataReader leitor = comandoSelecao.ExecuteReader();

            var mesas = new List<Mesa>();

            while (leitor.Read())
            {
                var mesa = ConverterParaMesa(leitor);

                mesas.Add(mesa);
            }

            conexaoComBanco.Close();
            return mesas;
        }
        private Mesa ConverterParaMesa(SqlDataReader leitor)
        {
            var mesa = new Mesa
                (
                    Convert.ToInt32(leitor["NUMERO"])!,
                    Convert.ToInt32(leitor["CAPACIDADE"])!
                );

            mesa.Id = Guid.Parse(leitor["ID"].ToString()!);


            return mesa;
        }

        private void ConfigurarParametrosMesa(SqlCommand comando, Mesa novoRegistro)
        {
            comando.Parameters.AddWithValue("ID", novoRegistro.Id);
            comando.Parameters.AddWithValue("NUMERO",   novoRegistro.Numero);
            comando.Parameters.AddWithValue("CAPACIDADE", novoRegistro.Capacidade);
            comando.Parameters.AddWithValue("ESTAOCUPADA", novoRegistro.EstaOcupada);
        }
    }
}
