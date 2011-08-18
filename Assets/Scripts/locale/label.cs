using System; 
public class label
{
	
	public static String opcoes = "Options";
	public static String tempoDeJogo = "Game Length";
	public static String minutos = "minutes";
	public static String toquesPorTime = "Actions per Team";
	public static String linguagem = "Language";
	public static String mostraReplay = "Show Replay";
	public static String modoAmistoso = "Friendly Match";
	public static String modoLiga = "League Mode";
	public static String modoCopa = "Cup Mode";
	public static String multiplayer = "Multiplayer";
	public static String comoJogar = "How to Play";
	public static String voltaAoMenu = "Back to Menu";
	public static String timeCasa = "Home Team";
	public static String timeFora = "Away Team";
	public static String sair = "Exit";
	public static String conectando = "Connecting";
	public static String esperando = "Waiting a connection";
	public static String conectar = "Connect to Server";
	public static String iniciarServidor = "Start Server";
	public static String cancelar = "Cancel";
	public static String mensagemChute = "Change the position of the goalkeeper and press R when ready";
	public static int cdLinguagem = 0;
	
	public static void trocaLinguagem(int codigoLinguagem){
		cdLinguagem = codigoLinguagem;
		switch(codigoLinguagem){
			case 0:
				enUS();
				break;
			case 1:
				ptBR();
				break;
		}
	}
	
	public static void enUS ()
	{
		opcoes = "Options";
		tempoDeJogo = "Game Length";
		minutos = "minutes";
		linguagem = "Language";
		mostraReplay = "Show Replay";
		modoAmistoso = "Friendly Match";
		modoLiga = "League Mode";
		modoCopa = "Cup Mode";
		multiplayer = "Multiplayer";
		comoJogar = "How to Play";
		voltaAoMenu = "Back to Menu";
		sair = "Exit";
		toquesPorTime = "Actions per Team";
		timeCasa = "Home Team";
		timeFora = "Away Team";
		conectar = "Connect to Server";
		iniciarServidor = "Start Server";
		cancelar = "Cancel";
		conectando = "Connecting";
		esperando = "Waiting a connection";
		mensagemChute = "Change the position of the goalkeeper and press R when ready";
	}
	
	public static void ptBR ()
	{
		opcoes = "Opções";
		tempoDeJogo = "Duração do Jogo";
		minutos = "minutos";
		linguagem = "Linguagem";
		mostraReplay = "Mostra Replay";
		modoAmistoso = "Modo Amistoso";
		modoLiga = "Modo Liga";
		modoCopa = "Modo Copa";
		multiplayer = "Multiplayer";
		comoJogar = "Como Jogar";
		voltaAoMenu = "Voltar ao Menu";
		sair = "Sair";
		toquesPorTime = "Toques por Time";
		timeCasa = "Time de Casa";
		timeFora = "Time de Fora";
		conectar = "Conectar";
		iniciarServidor = "Iniciar Servidor";
		cancelar = "Cancelar";
		conectando = "Conectando";
		esperando = "Esperando conexão";
		mensagemChute = "Posicione o goleiro e aperte R quando estiver pronto";
	}
}

