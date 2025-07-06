using ControleDeBar.Dominio.ModuloGarcom;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace ControleDeBar.Infraestrutura.SqlServer.ModuloGarcom
{
    public class RepositorioGarcomEmSql : IRepositorioGarcom
    {
        private readonly string connectionString =
        "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=controleDeBarDb;Integrated Security=True";
        public void CadastrarRegistro(Garcom novoRegistro)
        {
            var sqlInserir =
             @"INSERT INTO [TBGARCOM]
                (
                    [ID],
                    [NOME],
                    [CPF]
                )
                VALUES
                (
                    @ID,
                    @NOME,
                    @CPF
                )";

            SqlConnection conexaoComBanco = new SqlConnection(connectionString);
            SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

            ConfigurarParametrosContato(comandoInsercao, novoRegistro);
            conexaoComBanco.Open();

            comandoInsercao.ExecuteNonQuery();
            conexaoComBanco.Close();
        }

        public bool EditarRegistro(Guid idRegistro, Garcom registroEditado)
        {
            var sqlEditar =
              @"UPDATE [TBGARCOM]
                SET
                    [NOME] = @NOME,
                    [CPF] = @CPF
                WHERE
                    [ID] = @ID";
            SqlConnection conexaoComBanco = new SqlConnection(connectionString);
            SqlCommand comandoEdicao = new SqlCommand(sqlEditar, conexaoComBanco);

            registroEditado.Id = idRegistro;

            ConfigurarParametrosContato(comandoEdicao, registroEditado);
            conexaoComBanco.Open();

            var linhasAfetadas = comandoEdicao.ExecuteNonQuery();
            conexaoComBanco.Close();

            return linhasAfetadas > 0;
        }

        public bool ExcluirRegistro(Guid idRegistro)
        {
            var Sqlexcluir =
             @"DELETE FROM [TBGARCOM]
                WHERE [ID] = @ID";

            SqlConnection conexaoComBanco = new SqlConnection(connectionString);
            SqlCommand comandoExclusao = new SqlCommand(Sqlexcluir, conexaoComBanco);

            comandoExclusao.Parameters.AddWithValue("ID", idRegistro);
            conexaoComBanco.Open();

            var linhasAfetadas = comandoExclusao.ExecuteNonQuery();
            conexaoComBanco.Close();

            return linhasAfetadas > 0;
        }

        public Garcom SelecionarRegistroPorId(Guid idRegistro)
        {
            var SelecionarRegistroPorId =
             @"SELECT
                    [ID],
                    [NOME], 
                    [CPF]
                FROM 
                    [TBGARCOM]
                WHERE 
                    [ID] = @ID";


            SqlConnection conexaoComBanco = new SqlConnection(connectionString);
            SqlCommand comandoSelecao = new SqlCommand(SelecionarRegistroPorId, conexaoComBanco);
            comandoSelecao.Parameters.AddWithValue("ID", idRegistro);
            conexaoComBanco.Open();
            SqlDataReader leitor = comandoSelecao.ExecuteReader();
            Garcom? garcom = null;

            if (leitor.Read())
            {
                garcom = ConverterParaGarcom(leitor);
            }
            return garcom;
        }

        public List<Garcom> SelecionarRegistros()
        {
            var sqlSelecionarTodos =
             @"SELECT
                    [ID],
                    [NOME], 
                    [CPF]
                FROM 
                    [TBGARCOM]";


            SqlConnection conexaoComBanco = new SqlConnection(connectionString);
            conexaoComBanco.Open();

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarTodos, conexaoComBanco);


            SqlDataReader leitor = comandoSelecao.ExecuteReader();

            var garcons = new List<Garcom>();

            while (leitor.Read())
            {
                var garcom = ConverterParaGarcom(leitor);

                garcons.Add(garcom);
            }

            conexaoComBanco.Close();
            return garcons;
        }
        private Garcom ConverterParaGarcom(SqlDataReader leitor)
        {
            var garcom = new Garcom
                (
                    Convert.ToString(leitor["NOME"])!,
                    Convert.ToString(leitor["CPF"])!
                );

            garcom.Id = Guid.Parse(leitor["ID"].ToString()!);


            return garcom;
        }

        private void ConfigurarParametrosContato(SqlCommand comando, Garcom garcom)
        {
            comando.Parameters.AddWithValue("ID", garcom.Id);
            comando.Parameters.AddWithValue("NOME", garcom.Nome);
            comando.Parameters.AddWithValue("CPF", garcom.Cpf);
        }
    }
}
