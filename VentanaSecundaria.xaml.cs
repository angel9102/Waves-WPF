using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Waves
{
    /// <summary>
    /// Lógica de interacción para VentanaSecundaria.xaml
    /// </summary>

    /* --- DELEGADOS --- */
    public delegate void Window_ClosingEventHandler(object sender, FlagsNuevaPartida e);
    public delegate void NewWave_ClickEventHandler(object sender, OleadaNumero e);

    public class FlagsNuevaPartida : EventArgs
    {
        public bool Jugador { get; set; }
        public bool Mapa { get; set; }
        public FlagsNuevaPartida(bool pj, bool map)
        {
            Jugador = pj;
            Mapa = map;
        }
    }

    public class OleadaNumero : RoutedEventArgs
    {
        public string LabelOleada { get; set; }
        public OleadaNumero(string numWave)
        {
            LabelOleada = numWave;
        }
    }

    public partial class VentanaSecundaria : Window
    {
        private Modelo modelo;

        // Escenario del juego
        private Canvas miLienzo;
        private Rectangle personaje, enemigo1, enemigo2;
        private Button cambioMapaButton, cambioMusicaButton;

        // Variables para la Secuencia de Ataque
        private int jugador;
        private DispatcherTimer timer;
        private bool eliminado1, eliminado2;

        // Indicador de oleada actual
        private int numWave=1;

        // Música de la fase de combate
        private MediaPlayer musicaCombate;
        private bool musicOff = false;


        /* --- EVENTOS (basados en los delegados) ---*/
        public event Window_ClosingEventHandler Window_Closing;
        public event NewWave_ClickEventHandler NewWave_Click;

        /* --- CONSTRUCTOR VentanaSecundaria --- */
        public VentanaSecundaria(ImageBrush escenarioBrush, ImageBrush personajeBrush, int personajeJugador, 
                                 ListView miLista, MediaPlayer musicaMenu)
        {
            InitializeComponent();

            // Instancia un canvas, lo asocia al de VentanaSecundaria y lo pinta con el escenario seleccionado 
            miLienzo = new Canvas();
            miLienzo = Lienzo;
            miLienzo.Background = escenarioBrush;

            // Instancia el modelo con la ListView alojada en el Panel de Estadísticas de MainWindow
            // El modelo será el encargado de dar valor a esta lista
            modelo = new Modelo(miLista);

            // Establece el personaje del jugador y sus ataques en los botones
            DatosJugador datos;
            datos = modelo.Generar_Jugador(personajeBrush, personajeJugador);

            personaje = datos.pj;
            Atk1_Enem1.Content = datos.nombreAtaque1;
            Atk2_Enem1.Content = datos.nombreAtaque2;
            Atk1_Enem2.Content = datos.nombreAtaque1;
            Atk2_Enem2.Content = datos.nombreAtaque2;

            jugador = personajeJugador;

            // Establece los enemigos
            enemigo1 = new Rectangle();
            enemigo2 = new Rectangle();
            modelo.Generar_Enemigos(enemigo1, enemigo2);

            // Posicionamiento de los personajes en el escenario
            modelo.Posicionar_Personajes(miLienzo, personaje, enemigo1, enemigo2);

            // Botones del canvas
            cambioMapaButton = CambioMapa;
            cambioMusicaButton = CambioMusica;

            // Musica de combate
            musicaMenu.Pause();
            musicaCombate = new MediaPlayer();
            musicaCombate.Open(new Uri("Sonidos/15.mp3", UriKind.Relative));
            musicaCombate.MediaEnded += new EventHandler(Media_Ended);
            musicaCombate.Play();
        }

        // Cambia aleatoriamente a uno de los mapas
        private void CambioMapa_Click(object sender, RoutedEventArgs e)
        {
            ImageBrush nuevoEscenarioBrush = modelo.Cambiar_Mapa();
            miLienzo.Background = nuevoEscenarioBrush;
        }

        // Cambia aleatoriamente a una de las canciones
        private void CambioMusica_Click(object sender, RoutedEventArgs e)
        {
            modelo.SiguienteCancion(musicaCombate);
        }

        // Cuando una de las canciones de combate termina, se reproduce aleatoriamente la siguiente
        private void Media_Ended(object sender, EventArgs e)
        {
            modelo.SiguienteCancion(musicaCombate);
        }

        // Control de la música de combate
        private void PararMusica_Click(object sender, RoutedEventArgs e)
        {
            ImageBrush musicOffBrush = new ImageBrush();
            Rectangle rectangle = new Rectangle
            {
                Width = 20,
                Height = 20
            };

            if (!musicOff)
            {
                musicaCombate.Pause();
                musicOffBrush.ImageSource = new BitmapImage(new Uri("Imagenes/musicoff.png", UriKind.Relative));
                rectangle.Fill = musicOffBrush;
                PararMusica.Content = rectangle;
                musicOff = true;
            }
            else
            {
                musicaCombate.Play();
                musicOffBrush.ImageSource = new BitmapImage(new Uri("Imagenes/musicon.png", UriKind.Relative));
                rectangle.Fill = musicOffBrush;
                PararMusica.Content = rectangle;
                musicOff = false;
            }
        }

        // Cierre de la VentanaSecundaria
        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();  
        }

        /* --- Evento para poder hacer una nueva selección al cerrar VentanaSecundaria --- */
        void OnWindow_Closing(object sender, EventArgs e)
        {
            bool pj = true;
            bool map = true;

            if (Window_Closing != null)
                Window_Closing(this, new FlagsNuevaPartida(pj, map));

            miLienzo.Children.Clear();
            musicaCombate.Pause();
        }

        /* --- Evento para actualizar el número de oleada al hacer click en Siguiente Oleada --- */
        //     Prepara el Panel de Estadísticas y el canvas para la siguiente oleada
        private void OnNewWave_Click(object sender, RoutedEventArgs e)
        {
            // Se actualiza el numero de oleada tanto del panel en MainWindow como en VentanaSecundaria
            numWave++;
            string waveLabelContent = "OLEADA: ";
            waveLabelContent += numWave.ToString();

            if (NewWave_Click != null)
                NewWave_Click(this, new OleadaNumero(waveLabelContent));

            waveLabelContent = "OLEADA ACTUAL: ";
            waveLabelContent += numWave.ToString();
            CurrentWave.Content = waveLabelContent;

            // Restablece el jugador
            modelo.RestablecerJugadorListView();

            // Establece los nuevos enemigos
            eliminado1 = false;
            eliminado2 = false;
            enemigo1 = new Rectangle();
            enemigo2 = new Rectangle();
            modelo.Generar_Enemigos(enemigo1, enemigo2);

            // Posicionamiento de los personajes en el escenario
            modelo.Posicionar_Personajes(miLienzo, enemigo1, enemigo2);

            // Restablece los ataques (botones e indicadores)
            Atk1_Enem1.IsEnabled = true;
            Atk2_Enem1.IsEnabled = true;
            Atk1_Enem2.IsEnabled = true;
            Atk2_Enem2.IsEnabled = true;
            atkF1.Visibility = Visibility.Visible;
            atkF2.Visibility = Visibility.Visible;
            atkF3.Visibility = Visibility.Visible;

            // Oculta el botón de Siguiente Oleada
            newWaveText.Visibility = Visibility.Hidden;
            newWave.Visibility = Visibility.Hidden;
        }

        // Este método desencadena la secuencia de ataque: Atacar_Enemigo -> Atacar_Jugador
        private void Atacar_Enemigo_Click(object sender, RoutedEventArgs e)
        {
            bool atk1, enem1;

            /* 
             * Este timer será utilizado para controlar el tiempo de aparición
             * de los sprites en pantalla, así como la habilitación de los botones de ataque
             * 
             * Ejemplo de secuencia de ataque:
             *  El jugador ataca e inicia aquí el timer, a los 1500ms se dispara
             *  el método del modelo invocado en el Tick, que elimina el sprite de su ataque
             *  y detiene el timer. Se vuelve a reactivar cuando acto seguido el enemigo ataca.
             *  Finalmente se detiene tras haber sido eliminado el sprite del ataque enemigo.
             */

            timer = new DispatcherTimer();
            timer.Tick += AnimacionPorTurno;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1500);
            timer.Start();

            // Se determina el botón de los ataques que se ha pulsado
            if (sender.Equals(Atk1_Enem1))
            {
                atk1 = true;
                enem1 = true;
                Atk1_Enem1.IsEnabled = false;
                Atk2_Enem1.IsEnabled = false;
                Atk1_Enem2.IsEnabled = false;
                Atk2_Enem2.IsEnabled = false;
                eliminado1 = modelo.AtacarAlEnemigo(miLienzo, enemigo1, jugador, atk1, enem1);
            }
            else if (sender.Equals(Atk2_Enem1))
            {
                atk1 = false;
                enem1 = true;
                Atk1_Enem1.IsEnabled = false;
                Atk2_Enem1.IsEnabled = false;
                Atk1_Enem2.IsEnabled = false;
                Atk2_Enem2.IsEnabled = false;
                eliminado1 = modelo.AtacarAlEnemigo(miLienzo, enemigo1, jugador, atk1, enem1);
            }
            else if (sender.Equals(Atk1_Enem2))
            {
                atk1 = true;
                enem1 = false;
                Atk1_Enem1.IsEnabled = false;
                Atk2_Enem1.IsEnabled = false;
                Atk1_Enem2.IsEnabled = false;
                Atk2_Enem2.IsEnabled = false;
                eliminado2 = modelo.AtacarAlEnemigo(miLienzo, enemigo2, jugador, atk1, enem1);
            }
            else if (sender.Equals(Atk2_Enem2))
            {
                atk1 = false;
                enem1 = false;
                Atk1_Enem1.IsEnabled = false;
                Atk2_Enem1.IsEnabled = false;
                Atk1_Enem2.IsEnabled = false;
                Atk2_Enem2.IsEnabled = false;
                eliminado2 = modelo.AtacarAlEnemigo(miLienzo, enemigo2, jugador, atk1, enem1);
            }

            // Este flag es utilizado para controlar la reactivación de los botones y eliminar el sprite 
            // del ataque enemigo. Se pone a true cuando la animacion del ataque enemigo ha terminado
            modelo.reactivarAtaques = false;

            // Este metodo resta visualmente los ataques fuertes disponibles del jugador
            modelo.RestarAtaquesFuertes(atkF1, atkF2, atkF3);
        }

        private void AnimacionPorTurno(object sender, EventArgs e)
        {
            modelo.SecuenciaAtaque(miLienzo, timer, Atk1_Enem1, Atk2_Enem1, Atk1_Enem2, Atk2_Enem2, eliminado1, eliminado2, personaje, cambioMapaButton, cambioMusicaButton, PararMusica, musicaCombate, newWave, newWaveText);
        }

        internal void Magia(Personaje enemigoSeleccionado)
        {
            modelo.Magia(enemigoSeleccionado);
        }
    }
}
