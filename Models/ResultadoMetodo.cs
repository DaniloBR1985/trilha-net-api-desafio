namespace TrilhaApiDesafio.Models
{
    public class ResultadoMetodo
    {
        public bool Sucesso { get; private set; }
        public string Mensagem { get; private set; }

        public static ResultadoMetodo Ok()
            => new(true, null);

        public static ResultadoMetodo Falha(string mensagem)
            => new(false, mensagem);

        private ResultadoMetodo(bool sucesso, string mensagem)
        {
            Sucesso = sucesso;
            Mensagem = mensagem;
        }
    }
}
