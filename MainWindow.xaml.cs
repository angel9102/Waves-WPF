using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

/* Imágenes:
 * - Sprites de los personajes https://craftpix.net/freebies/free-golems-chibi-2d-game-sprites/
 * - Sprites de los ataques procedentes del portal https://www.pngguru.com/ 
 * - Escenarios https://assetstore.unity.com/packages/2d/environments/17-great-battle-backgrounds-156486
 * - Imagen y sonido de fin de partida pertenece a la saga Dark Souls de From Software
 * - Resto de imágenes utilizadas provienen de diferentes fuentes de internet
 * 
 * Sonidos:
 * - Música del menú y de combate https://archive.org/details/jamendo-097169
 * - Efectos de sonido de los ataques https://bellblitzking.itch.io/pokemon-sound-collection
 * 
 * Aplicación desarrollada por Ángel Guerrero López, Universidad de Salamanca, 2019
 * 
 * */

namespace Waves
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Comprobaciones para habilitar o deshabilitar comienzo del juego
        private bool personajeFlag = true, escenarioFlag = true;

        private ImageBrush personajeBrush, escenarioBrush;
        private int personajeJugador;

        VentanaSecundaria miVentana;
        private ListView miLista;

        private MediaPlayer musicaMenu;
        private bool musicOff = false;


        public MainWindow()
        {
            InitializeComponent();

            miLista = new ListView();
            miLista = listaEstadisticas;

            musicaMenu = new MediaPlayer();
            musicaMenu.Open(new Uri("Sonidos/34menu.mp3", UriKind.Relative));
            musicaMenu.MediaEnded += new EventHandler(Media_Ended);
            musicaMenu.Play();

        }


        /*
         * Los dos metodos siguientes modifican elementos visuales de la MainWindow.
         * 
         * Personaje_Click() determinará el personaje seleccionado y 
         * Escenario_Click() establecerá el escenario, así mismo,
         * ambos métodos controlarán las posibilidades de la selección
         * y aspectos visuales de la misma.
         * 
         */
        private void Personaje_Click(object sender, RoutedEventArgs e)
        {
            ImageBrush myBrush = new ImageBrush();
            LabelNuevaSeleccion.Visibility = Visibility.Hidden;

            if (personajeBrush == null || personajeFlag)
            {
                if (sender.Equals(Suiton))
                {
                    Suiton.BorderBrush = Brushes.Black;
                    Suiton.BorderThickness = new Thickness(3, 3, 3, 3);
                    Doton.BorderThickness = new Thickness();
                    Katon.BorderThickness = new Thickness();

                    // Establece la brocha con la que se rellena la figura del personaje (sprite)
                    myBrush.ImageSource = new BitmapImage(new Uri("Imagenes/personajes/1_Suiton.png", UriKind.Relative));
                    personajeBrush = myBrush;

                    // Este valor se pasa en el constructor de Personaje para determinar el personaje del jugador
                    personajeJugador = 0; 
                }

                if (sender.Equals(Doton))
                {
                    Doton.BorderBrush = Brushes.Black;
                    Doton.BorderThickness = new Thickness(3, 3, 3, 3);
                    Suiton.BorderThickness = new Thickness();
                    Katon.BorderThickness = new Thickness();

                    myBrush.ImageSource = new BitmapImage(new Uri("Imagenes/personajes/2_Doton.png", UriKind.Relative));
                    personajeBrush = myBrush;

                    personajeJugador = 1;
                }

                if (sender.Equals(Katon))
                {
                    Katon.BorderBrush = Brushes.Black;
                    Katon.BorderThickness = new Thickness(3, 3, 3, 3);
                    Suiton.BorderThickness = new Thickness();
                    Doton.BorderThickness = new Thickness();

                    myBrush.ImageSource = new BitmapImage(new Uri("Imagenes/personajes/3_Katon.png", UriKind.Relative));
                    personajeBrush = myBrush;

                    personajeJugador = 2;
                }

                if (escenarioBrush != null)
                {
                    Start.IsEnabled = true;
                }
            }
        }
        
        private void Escenario_Click(object sender, RoutedEventArgs e)
        {
            ImageBrush myBrush = new ImageBrush();
            LabelNuevaSeleccion.Visibility = Visibility.Hidden;

            if (escenarioBrush == null || escenarioFlag)
            {
                if (sender.Equals(Cave))
                {
                    Cave.BorderBrush = Brushes.Black;
                    Cave.BorderThickness = new Thickness(3, 3, 3, 3);
                    Desert.BorderThickness = new Thickness();
                    Forest.BorderThickness = new Thickness();
                    Hell.BorderThickness = new Thickness();
                    Japan.BorderThickness = new Thickness();
                    Mountain.BorderThickness = new Thickness();
                    Mystic.BorderThickness = new Thickness();

                    // Establece la brocha para el background del canvas en VentanaSecundaria (escenario)
                    myBrush.ImageSource = new BitmapImage(new Uri("Imagenes/escenarios/cave.jpg", UriKind.Relative));
                    escenarioBrush = myBrush;
                }

                if (sender.Equals(Desert))
                {
                    Desert.BorderBrush = Brushes.Black;
                    Desert.BorderThickness = new Thickness(3, 3, 3, 3);
                    Cave.BorderThickness = new Thickness();
                    Forest.BorderThickness = new Thickness();
                    Hell.BorderThickness = new Thickness();
                    Japan.BorderThickness = new Thickness();
                    Mountain.BorderThickness = new Thickness();
                    Mystic.BorderThickness = new Thickness();

                    myBrush.ImageSource = new BitmapImage(new Uri("Imagenes/escenarios/desert.jpg", UriKind.Relative));
                    escenarioBrush = myBrush;
                }

                if (sender.Equals(Forest))
                {
                    Forest.BorderBrush = Brushes.Black;
                    Forest.BorderThickness = new Thickness(3, 3, 3, 3);
                    Cave.BorderThickness = new Thickness();
                    Desert.BorderThickness = new Thickness();
                    Hell.BorderThickness = new Thickness();
                    Japan.BorderThickness = new Thickness();
                    Mountain.BorderThickness = new Thickness();
                    Mystic.BorderThickness = new Thickness();

                    myBrush.ImageSource = new BitmapImage(new Uri("Imagenes/escenarios/forest.jpg", UriKind.Relative));
                    escenarioBrush = myBrush;
                }

                if (sender.Equals(Hell))
                {
                    Hell.BorderBrush = Brushes.Black;
                    Hell.BorderThickness = new Thickness(3, 3, 3, 3);
                    Cave.BorderThickness = new Thickness();
                    Desert.BorderThickness = new Thickness();
                    Forest.BorderThickness = new Thickness();
                    Japan.BorderThickness = new Thickness();
                    Mountain.BorderThickness = new Thickness();
                    Mystic.BorderThickness = new Thickness();

                    myBrush.ImageSource = new BitmapImage(new Uri("Imagenes/escenarios/hell.jpg", UriKind.Relative));
                    escenarioBrush = myBrush;
                }

                if (sender.Equals(Japan))
                {
                    Japan.BorderBrush = Brushes.Black;
                    Japan.BorderThickness = new Thickness(3, 3, 3, 3);
                    Cave.BorderThickness = new Thickness();
                    Desert.BorderThickness = new Thickness();
                    Forest.BorderThickness = new Thickness();
                    Hell.BorderThickness = new Thickness();
                    Mountain.BorderThickness = new Thickness();
                    Mystic.BorderThickness = new Thickness();

                    myBrush.ImageSource = new BitmapImage(new Uri("Imagenes/escenarios/japan.jpg", UriKind.Relative));
                    escenarioBrush = myBrush;
                }

                if (sender.Equals(Mountain))
                {
                    Mountain.BorderBrush = Brushes.Black;
                    Mountain.BorderThickness = new Thickness(3, 3, 3, 3);
                    Cave.BorderThickness = new Thickness();
                    Desert.BorderThickness = new Thickness();
                    Forest.BorderThickness = new Thickness();
                    Hell.BorderThickness = new Thickness();
                    Japan.BorderThickness = new Thickness();
                    Mystic.BorderThickness = new Thickness();

                    myBrush.ImageSource = new BitmapImage(new Uri("Imagenes/escenarios/mountain.jpg", UriKind.Relative));
                    escenarioBrush = myBrush;
                }

                if (sender.Equals(Mystic))
                {
                    Mystic.BorderBrush = Brushes.Black;
                    Mystic.BorderThickness = new Thickness(3, 3, 3, 3);
                    Cave.BorderThickness = new Thickness();
                    Desert.BorderThickness = new Thickness();
                    Forest.BorderThickness = new Thickness();
                    Hell.BorderThickness = new Thickness();
                    Japan.BorderThickness = new Thickness();
                    Mountain.BorderThickness = new Thickness();

                    myBrush.ImageSource = new BitmapImage(new Uri("Imagenes/escenarios/mystic.jpg", UriKind.Relative));
                    escenarioBrush = myBrush;
                }

                if (personajeBrush != null)
                {
                    Start.IsEnabled = true;
                }
            }
        }


        /*
         * Se corresponde con el botón de Comenzar Juego: 
         * "deshabilita" los botones de selección de personaje y escenario,
         * inicia la VentanaSecundaria,
         * hace visible el panel con las estadísticas de la partida y
         * controla el cierre de la VentanaSecundaria con un gestor para tal evento.
         * 
         */
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Start.IsEnabled = false;
            personajeFlag = false;
            escenarioFlag = false;
            
            // Inicializa la VentanaSecundaria con las brochas de escenario y jugador
            miVentana = new VentanaSecundaria(escenarioBrush, personajeBrush, personajeJugador, miLista, musicaMenu);
            miVentana.Owner = this;
            miVentana.Show();

            // Panel de estadísticas
            panelEstadisticas.Visibility = Visibility.Visible;

            // Lista de invocación del evento Window_Closing y NewWave_Click basados en sus delegados
            miVentana.Window_Closing += MiVentana_Window_Closing;
            miVentana.NewWave_Click += MiVentana_NewWave_Click;

            PararMusica.IsEnabled = false;
        }

        
        // Gestor del evento Window_Closing: establece los valores de ambos flags a true 
        // para poder realizar una nueva selección 
        private void MiVentana_Window_Closing(object sender, FlagsNuevaPartida e)
        {
            personajeFlag = e.Jugador;
            escenarioFlag = e.Mapa;
            LabelNuevaSeleccion.Visibility = Visibility.Visible;

            // Reseteo de oleadas al cerrar y comienza a sonar la música del menú si no está desactivada
            panelEstadisticas.Visibility = Visibility.Hidden;
            labelOleada.Content = "OLEADA: 1";

            PararMusica.IsEnabled = true;
            if (!musicOff)
            {
                musicaMenu.Open(new Uri("Sonidos/34menu.mp3", UriKind.Relative));
                musicaMenu.Play();
            }
        }

         
        // Gestor del evento NewWave_Click: cambia el contenido de la etiqueta 
        // del panel de estadísticas que indica el número de oleada actual
        private void MiVentana_NewWave_Click(object sender, OleadaNumero e)
        {
            labelOleada.Content = e.LabelOleada;
        }

        
        // Botón Mágico: obtiene el personaje seleccionado de la lista para pasárselo 
        // al método del modelo encargado de eliminarlo de la colección
        private void Magia_Click(object sender, RoutedEventArgs e)
        {
            Personaje enemigoSeleccionado = (Personaje)miLista.SelectedItem;

            if (enemigoSeleccionado != null)
            {
                miVentana.Magia(enemigoSeleccionado);
            }
        }


        // Control de la música del menú principal
        private void PararMusica_Click(object sender, RoutedEventArgs e)
        {
            ImageBrush musicOffBrush = new ImageBrush();
            Rectangle rectangle = new Rectangle();

            if (!musicOff)
            {
                musicaMenu.Pause();
                musicOffBrush.ImageSource = new BitmapImage(new Uri("Imagenes/musicoff.png", UriKind.Relative));
                rectangle.Fill = musicOffBrush;
                rectangle.Width = 30;
                rectangle.Height = 30;
                PararMusica.Content = rectangle;
                musicOff = true;
            }
            else
            {
                musicaMenu.Play();
                musicOffBrush.ImageSource = new BitmapImage(new Uri("Imagenes/musicon.png", UriKind.Relative));
                rectangle.Fill = musicOffBrush;
                rectangle.Width = 30;
                rectangle.Height = 30;
                PararMusica.Content = rectangle;
                musicOff = false;
            } 
        }


        // Reinicio de la música del menú principal cuando ésta ha terminado 
        private void Media_Ended(object sender, EventArgs e)
        {
            musicaMenu.Position = TimeSpan.Zero;
            musicaMenu.Play();
        }


        // Cierre de la MainWindow
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
