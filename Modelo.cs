using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Waves
{
    public struct DatosJugador
    {
        public Rectangle pj;
        public string nombreAtaque1;
        public string nombreAtaque2;
    }

    public class Modelo
    {
        private enum Escenarios { CAVE, DESERT, FOREST, HELL, JAPAN, MOUNTAIN, MYSTIC }
        private Personajes tipoEnemigo1, tipoEnemigo2;

        ObservableCollection<Personaje> listaPersonajes = new ObservableCollection<Personaje>();
        private Personaje jugador, enemigo1, enemigo2;
        private TextBlock personajeText, enemigo1Text, enemigo2Text;
        private Rectangle ataqueSprite, enemAtkSprite;

        private bool eliminado, jugadorEliminado = false;
        private bool enem1alive = true, enem2alive = true;
        public bool reactivarAtaques = false;

        private int usosAtk2 = 3;
        private int numWave = 1;
        private int atk1Jugador = 25, atk2Jugador = 35;
        private int atk1Enemigo = 10, atk2Enemigo = 15;

        // Buff y nerf por elemento
        private const int ELEMENT_BUFF = 5;
        private const int ELEMENT_NERF = 5;

        private MediaPlayer ataqueSFX = new MediaPlayer();

        public Modelo(ListView miLista)
        {
            // El control ListView toma los datos de la colección: se vincula el modelo con la vista
            miLista.ItemsSource = listaPersonajes;
        }

        /* 
         *  Genera al jugador en escena y establece el nombre para los botones de ataque
         * 
         *  personajeBrush toma valor tras la Seleccion de Personaje
         *  realizada en MainWindow. Dicho valor se pasa al constructor
         *  de la VentanaSecundaria e invoca este método del Modelo,
         *  donde se utilizará para rellenar la figura del personaje.
         * 
         *  Valor de retorno: una estructura con la figura del personaje seleccionado
         *                    y los nombres de los ataques de dicho personaje
         * 
         */
        public DatosJugador Generar_Jugador(ImageBrush personajeBrush, int personajeJugador)
        {
            int tipoPJ = 0;
            DatosJugador datos;

            datos.pj = new Rectangle
            {
                Fill = personajeBrush,
                Height = 140,
                Width = 130
            };

            jugador = new Personaje(tipoPJ, personajeJugador);
            listaPersonajes.Add(jugador);

            datos.nombreAtaque1 = jugador.Ataque1;
            datos.nombreAtaque2 = jugador.Ataque2;

            return datos;
        }

         
        // Establece dos enemigos de forma aleatoria para que sean dibujados en el escenario
        // Añade los enemigos a la colección de personajes
        public void Generar_Enemigos(Rectangle enem1, Rectangle enem2)
        {
            ImageBrush myEnem1Brush = new ImageBrush();
            ImageBrush myEnem2Brush = new ImageBrush();
            Random rand;
            int tipoPJ;

            // Genera primer enemigo
            tipoPJ = 1;
            rand = new Random();
            tipoEnemigo1 = (Personajes)rand.Next(0, 2);

            if (tipoEnemigo1 == Personajes.SUITON)
            {
                myEnem1Brush.ImageSource = new BitmapImage(new Uri("Imagenes/personajes/1_Suiton_Inv.png", UriKind.Relative));
                enemigo1 = new Personaje(tipoPJ, (int)tipoEnemigo1);
                enemigo1.PotenciaAtk1 = atk1Enemigo;
                enemigo1.PotenciaAtk2 = atk2Enemigo;
                listaPersonajes.Add(enemigo1);
            }

            else if (tipoEnemigo1 == Personajes.DOTON)
            {
                myEnem1Brush.ImageSource = new BitmapImage(new Uri("Imagenes/personajes/2_Doton_Inv.png", UriKind.Relative));
                enemigo1 = new Personaje(tipoPJ, (int)tipoEnemigo1);
                enemigo1.PotenciaAtk1 = atk1Enemigo;
                enemigo1.PotenciaAtk2 = atk2Enemigo;
                listaPersonajes.Add(enemigo1);
            }

            enem1.Fill = myEnem1Brush;
            enem1.Height = 140;
            enem1.Width = 130;

            // Genera segundo enemigo
            tipoPJ = 2;
            rand = new Random();
            tipoEnemigo2 = (Personajes)rand.Next(1, 3);

            if (tipoEnemigo2 == Personajes.DOTON)
            {
                myEnem2Brush.ImageSource = new BitmapImage(new Uri("Imagenes/personajes/2_Doton_Inv.png", UriKind.Relative));
                enemigo2 = new Personaje(tipoPJ, (int)tipoEnemigo2);
                enemigo2.PotenciaAtk1 = atk1Enemigo;
                enemigo2.PotenciaAtk2 = atk2Enemigo;
                listaPersonajes.Add(enemigo2);
            }

            else if (tipoEnemigo2 == Personajes.KATON)
            {
                myEnem2Brush.ImageSource = new BitmapImage(new Uri("Imagenes/personajes/3_Katon_Inv.png", UriKind.Relative));
                enemigo2 = new Personaje(tipoPJ, (int)tipoEnemigo2);
                enemigo2.PotenciaAtk1 = atk1Enemigo;
                enemigo2.PotenciaAtk2 = atk2Enemigo;
                listaPersonajes.Add(enemigo2);
            }

            enem2.Fill = myEnem2Brush;
            enem2.Height = 140;
            enem2.Width = 130;
        }
        

        // Posiciona los personajes y sus etiquetas en el escenario (primera oleada)
        public void Posicionar_Personajes(Canvas miLienzo, Rectangle personaje, Rectangle enemigo1fig, Rectangle enemigo2fig)
        {
            Canvas.SetLeft(personaje, 190);
            Canvas.SetTop(personaje, 80);

            personajeText = new TextBlock
            {
                Text = "JUGADOR",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                Opacity = 0.8
            };

            Canvas.SetLeft(personajeText, 225);
            Canvas.SetTop(personajeText, 200);

            Canvas.SetLeft(enemigo1fig, 450);
            Canvas.SetTop(enemigo1fig, 55);

            enemigo1Text = new TextBlock
            {
                Text = "ENEMIGO 1",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                Opacity = 0.8
            };

            Canvas.SetLeft(enemigo1Text, 485);
            Canvas.SetTop(enemigo1Text, 175);

            Canvas.SetLeft(enemigo2fig, 575);
            Canvas.SetTop(enemigo2fig, 105);

            enemigo2Text = new TextBlock
            {
                Text = "ENEMIGO 2",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                Opacity = 0.8
            };

            Canvas.SetLeft(enemigo2Text, 610);
            Canvas.SetTop(enemigo2Text, 225);

            miLienzo.Children.Add(personaje);
            miLienzo.Children.Add(personajeText);
            miLienzo.Children.Add(enemigo1fig);
            miLienzo.Children.Add(enemigo1Text);
            miLienzo.Children.Add(enemigo2fig);
            miLienzo.Children.Add(enemigo2Text);
        }

        // (Sobrecarga) Posiciona los enemigos y sus etiquetas en el escenario (siguientes oleadas)
        public void Posicionar_Personajes(Canvas miLienzo, Rectangle enemigo1fig, Rectangle enemigo2fig)
        {
            Canvas.SetLeft(enemigo1fig, 450);
            Canvas.SetTop(enemigo1fig, 55);

            enemigo1Text = new TextBlock
            {
                Text = "ENEMIGO 1",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                Opacity = 0.8
            };

            Canvas.SetLeft(enemigo1Text, 485);
            Canvas.SetTop(enemigo1Text, 175);

            Canvas.SetLeft(enemigo2fig, 575);
            Canvas.SetTop(enemigo2fig, 105);

            enemigo2Text = new TextBlock
            {
                Text = "ENEMIGO 2",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                Opacity = 0.8
            };

            Canvas.SetLeft(enemigo2Text, 610);
            Canvas.SetTop(enemigo2Text, 225);

            miLienzo.Children.Add(enemigo1fig);
            miLienzo.Children.Add(enemigo1Text);
            miLienzo.Children.Add(enemigo2fig);
            miLienzo.Children.Add(enemigo2Text);
        }


        public bool AtacarAlEnemigo(Canvas miLienzo, Rectangle enemfig, int jugador, bool atk1, bool enem1)
        {
            // Control de aspectos visuales y de mecánicas del juego
            bool burbuja = false, hidropulso = false, caos = false;
            bool elementBuff = false;
            bool elementNerf = false;

            // Para generar el sprite del ataque del jugador
            ataqueSprite = new Rectangle();
            ImageBrush myAtkBrush = new ImageBrush();
            string uri = "Imagenes/ataques/";
            string uriSFX = "Sonidos/";

            // Bloque para el enemigo 1
            if (enem1)
            {
                // Bloque para el ataque 1
                if (atk1)
                {
                    switch ((Personajes)jugador)
                    {
                        case Personajes.SUITON:
                            uri += "burbuja.png";
                            burbuja = true;
                            if (enemigo1.Nombre.Equals("Doton"))
                                elementNerf = true;
                            break;
                        case Personajes.DOTON:
                            uri += "hojaafilada.png";
                            if (enemigo1.Nombre.Equals("Suiton"))
                                elementBuff = true;
                            break;
                        case Personajes.KATON:
                            uri += "ascua.png";
                            if (enemigo1.Nombre.Equals("Doton"))
                                elementBuff = true;
                            if (enemigo1.Nombre.Equals("Suiton"))
                                elementNerf = true;
                            break;
                    }

                    if (elementBuff)
                    {
                        if (enemigo1.Vida > atk1Jugador + ELEMENT_BUFF)
                        {
                            enemigo1.Vida -= atk1Jugador + ELEMENT_BUFF;
                            eliminado = false;
                        }
                        else
                        {
                            listaPersonajes.Remove(enemigo1);
                            miLienzo.Children.Remove(enemfig);
                            miLienzo.Children.Remove(enemigo1Text);
                            eliminado = true;
                            enem1alive = false;
                        }
                    }
                    else if (elementNerf)
                    {
                        if (enemigo1.Vida > atk1Jugador - ELEMENT_NERF)
                        {
                            enemigo1.Vida -= atk1Jugador - ELEMENT_NERF;
                            eliminado = false;
                        }
                        else
                        {
                            listaPersonajes.Remove(enemigo1);
                            miLienzo.Children.Remove(enemfig);
                            miLienzo.Children.Remove(enemigo1Text);
                            eliminado = true;
                            enem1alive = false;
                        }
                    }
                    else
                    {
                        if (enemigo1.Vida > atk1Jugador)
                        {
                            enemigo1.Vida -= atk1Jugador;
                            eliminado = false;
                        }
                        else
                        {
                            listaPersonajes.Remove(enemigo1);
                            miLienzo.Children.Remove(enemfig);
                            miLienzo.Children.Remove(enemigo1Text);
                            eliminado = true;
                            enem1alive = false;
                        }
                    }
                }
                // Bloque para el ataque 2
                else
                {
                    switch ((Personajes)jugador)
                    {
                        case Personajes.SUITON:
                            uri += "hidropulso.png";
                            hidropulso = true;
                            if (enemigo1.Nombre.Equals("Doton"))
                                elementNerf = true;
                            break;
                        case Personajes.DOTON:
                            uri += "rayosolar.png";
                            if (enemigo1.Nombre.Equals("Suiton"))
                                elementBuff = true;
                            break;
                        case Personajes.KATON:
                            uri += "caos.png";
                            caos = true;
                            if (enemigo1.Nombre.Equals("Doton"))
                                elementBuff = true;
                            if (enemigo1.Nombre.Equals("Suiton"))
                                elementNerf = true;
                            break;
                    }

                    if (elementBuff)
                    {
                        if (enemigo1.Vida > atk2Jugador + ELEMENT_BUFF)
                        {
                            enemigo1.Vida -= atk2Jugador + ELEMENT_BUFF;
                            eliminado = false;
                        }
                        else
                        {
                            listaPersonajes.Remove(enemigo1);
                            miLienzo.Children.Remove(enemfig);
                            miLienzo.Children.Remove(enemigo1Text);
                            eliminado = true;
                            enem1alive = false;
                        }
                    }
                    else if (elementNerf)
                    {
                        if (enemigo1.Vida > atk2Jugador - ELEMENT_NERF)
                        {
                            enemigo1.Vida -= atk2Jugador - ELEMENT_NERF;
                            eliminado = false;
                        }
                        else
                        {
                            listaPersonajes.Remove(enemigo1);
                            miLienzo.Children.Remove(enemfig);
                            miLienzo.Children.Remove(enemigo1Text);
                            eliminado = true;
                            enem1alive = false;
                        }
                    }
                    else
                    {
                        if (enemigo1.Vida > atk2Jugador)
                        {
                            enemigo1.Vida -= atk2Jugador;
                            eliminado = false;
                        }
                        else
                        {
                            listaPersonajes.Remove(enemigo1);
                            miLienzo.Children.Remove(enemfig);
                            miLienzo.Children.Remove(enemigo1Text);
                            eliminado = true;
                            enem1alive = false;
                        }
                    }

                    usosAtk2--;
                }
            }

            // Bloque para el enemigo 2
            else
            {
                // Bloque para el ataque 1
                if (atk1)
                {
                    switch ((Personajes)jugador)
                    {
                        case Personajes.SUITON:
                            uri += "burbuja.png";
                            burbuja = true;
                            if (enemigo2.Nombre.Equals("Katon"))
                                elementBuff = true;
                            if (enemigo2.Nombre.Equals("Doton"))
                                elementNerf = true;
                            break;
                        case Personajes.DOTON:
                            uri += "hojaafilada.png";
                            if (enemigo2.Nombre.Equals("Katon"))
                                elementNerf = true;
                            break;
                        case Personajes.KATON:
                            uri += "ascua.png";
                            if (enemigo2.Nombre.Equals("Doton"))
                                elementBuff = true;
                            break;
                    }

                    if (elementBuff)
                    {
                        if (enemigo2.Vida > atk1Jugador + ELEMENT_BUFF)
                        {
                            enemigo2.Vida -= atk1Jugador + ELEMENT_BUFF;
                            eliminado = false;
                        }
                        else
                        {
                            listaPersonajes.Remove(enemigo2);
                            miLienzo.Children.Remove(enemfig);
                            miLienzo.Children.Remove(enemigo2Text);
                            eliminado = true;
                            enem2alive = false;
                        }
                    }
                    else if (elementNerf)
                    {
                        if (enemigo2.Vida > atk1Jugador - ELEMENT_NERF)
                        {
                            enemigo2.Vida -= atk1Jugador - ELEMENT_NERF;
                            eliminado = false;
                        }
                        else
                        {
                            listaPersonajes.Remove(enemigo2);
                            miLienzo.Children.Remove(enemfig);
                            miLienzo.Children.Remove(enemigo2Text);
                            eliminado = true;
                            enem2alive = false;
                        }
                    }
                    else
                    {
                        if (enemigo2.Vida > atk1Jugador)
                        {
                            enemigo2.Vida -= atk1Jugador;
                            eliminado = false;
                        }
                        else
                        {
                            listaPersonajes.Remove(enemigo2);
                            miLienzo.Children.Remove(enemfig);
                            miLienzo.Children.Remove(enemigo2Text);
                            eliminado = true;
                            enem2alive = false;
                        }
                    }
                }
                // Bloque para el ataque 2
                else
                {
                    switch ((Personajes)jugador)
                    {
                        case Personajes.SUITON:
                            uri += "hidropulso.png";
                            hidropulso = true;
                            if (enemigo2.Nombre.Equals("Katon"))
                                elementBuff = true;
                            if (enemigo2.Nombre.Equals("Doton"))
                                elementNerf = true;
                            break;
                        case Personajes.DOTON:
                            uri += "rayosolar.png";
                            if (enemigo2.Nombre.Equals("Katon"))
                                elementNerf = true;
                            break;
                        case Personajes.KATON:
                            uri += "caos.png";
                            caos = true;
                            if (enemigo2.Nombre.Equals("Doton"))
                                elementBuff = true;
                            break;
                    }

                    if (elementBuff)
                    {
                        if (enemigo2.Vida > atk2Jugador + ELEMENT_BUFF)
                        {
                            enemigo2.Vida -= atk2Jugador + ELEMENT_BUFF;
                            eliminado = false;
                        }
                        else
                        {
                            listaPersonajes.Remove(enemigo2);
                            miLienzo.Children.Remove(enemfig);
                            miLienzo.Children.Remove(enemigo2Text);
                            eliminado = true;
                            enem2alive = false;
                        }
                    }
                    else if (elementNerf)
                    {
                        if (enemigo2.Vida > atk2Jugador - ELEMENT_NERF)
                        {
                            enemigo2.Vida -= atk2Jugador - ELEMENT_NERF;
                            eliminado = false;
                        }
                        else
                        {
                            listaPersonajes.Remove(enemigo2);
                            miLienzo.Children.Remove(enemfig);
                            miLienzo.Children.Remove(enemigo2Text);
                            eliminado = true;
                            enem2alive = false;
                        }
                    }
                    else
                    {
                        if (enemigo2.Vida > atk2Jugador)
                        {
                            enemigo2.Vida -= atk2Jugador;
                            eliminado = false;
                        }
                        else
                        {
                            listaPersonajes.Remove(enemigo2);
                            miLienzo.Children.Remove(enemfig);
                            miLienzo.Children.Remove(enemigo2Text);
                            eliminado = true;
                            enem2alive = false;
                        }
                    }

                    usosAtk2--;
                }
            }

            // Animacion del ataque
            myAtkBrush.ImageSource = new BitmapImage(new Uri(uri, UriKind.Relative));
            ataqueSprite.Fill = myAtkBrush;

            // Algunos ajustes para los sprites
            if (burbuja)
            {
                ataqueSprite.Height = 60;
                ataqueSprite.Width = 60;
                if (enem1)
                {
                    Canvas.SetLeft(ataqueSprite, 485);
                    Canvas.SetTop(ataqueSprite, 110);
                }
                else
                {
                    Canvas.SetLeft(ataqueSprite, 610);
                    Canvas.SetTop(ataqueSprite, 160);
                }
            }
            else if (hidropulso)
            {
                ataqueSprite.Height = 85;
                ataqueSprite.Width = 85;
                if (enem1)
                {
                    Canvas.SetLeft(ataqueSprite, 475);
                    Canvas.SetTop(ataqueSprite, 90);
                }
                else
                {
                    Canvas.SetLeft(ataqueSprite, 600);
                    Canvas.SetTop(ataqueSprite, 140);
                }
            }
            else if (caos)
            {
                ataqueSprite.Height = 90;
                ataqueSprite.Width = 90;
                if (enem1)
                {
                    Canvas.SetLeft(ataqueSprite, 470);
                    Canvas.SetTop(ataqueSprite, 90);
                }
                else
                {
                    Canvas.SetLeft(ataqueSprite, 595);
                    Canvas.SetTop(ataqueSprite, 135);
                }
            }
            else
            {
                ataqueSprite.Height = 80;
                ataqueSprite.Width = 80;
                if (enem1)
                {
                    Canvas.SetLeft(ataqueSprite, 475);
                    Canvas.SetTop(ataqueSprite, 100);
                }
                else
                {
                    Canvas.SetLeft(ataqueSprite, 600);
                    Canvas.SetTop(ataqueSprite, 150);
                }
            }

            miLienzo.Children.Add(ataqueSprite);

            // Efectos de sonido para los ataques
            if (uri.Contains("burbuja"))
                uriSFX += "burbuja.mp3";
            else if (uri.Contains("hidropulso"))
                uriSFX += "hidropulso.mp3";
            else if (uri.Contains("hojaafilada"))
                uriSFX += "hojaafilada.mp3";
            else if (uri.Contains("rayosolar"))
                uriSFX += "rayosolar.mp3";
            else if (uri.Contains("ascua"))
                uriSFX += "ascua.mp3";
            else if (uri.Contains("caos"))
                uriSFX += "caos.wav";

            ataqueSFX.Open(new Uri(uriSFX, UriKind.Relative));
            ataqueSFX.Play();

            return eliminado;
        }

        public void RestarAtaquesFuertes(TextBlock atkF1, TextBlock atkF2, TextBlock atkF3)
        {
            switch (usosAtk2)
            {
                case 2:
                    atkF3.Visibility = Visibility.Hidden;
                    break;
                case 1:
                    atkF2.Visibility = Visibility.Hidden;
                    break;
                case 0:
                    atkF1.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }
        }

        // Este método se ejecuta automáticamente tras eliminarse el sprite del ataque del jugador
        public bool AtacarAlJugador(Canvas miLienzo, Rectangle jugadorfig, DispatcherTimer timer)
        {
            Random randEnemigo = new Random();
            Random randAtaque = new Random();

            int enemigo = randEnemigo.Next(0, 2);
            int ataque = randAtaque.Next(0, 10);

            bool elementBuff = false;
            bool elementNerf = false;
            bool burbuja = false, hidropulso = false, caos = false;

            ImageBrush enemAtkBrush = new ImageBrush();
            enemAtkSprite = new Rectangle();

            string uri = "Imagenes/ataques/";
            string uriSFX = "Sonidos/";

            timer.Start();

            // Se determina si ataca un enemigo u otro
            if(enem1alive && enem2alive)
            {
                enemigo = randEnemigo.Next(0, 2);
            }
            else if (enem1alive && !enem2alive)
            {
                enemigo = 0;
            }
            else if (!enem1alive && enem2alive)
            {
                enemigo = 1;
            }

            // Control del último ataque de la ronda
            if(enem1alive || enem2alive)
            {
                // Ataca enemigo1
                switch (enemigo)
                {
                    case 0:
                        switch (tipoEnemigo1)
                        {
                            case Personajes.SUITON:
                                if (jugador.Nombre.Equals("Katon"))
                                    elementBuff = true;
                                else if (jugador.Nombre.Equals("Doton"))
                                    elementNerf = true;
                                // Se determina si es el ataque1 o el ataque2
                                // Ataque1 (mayor probabilidad que ataque2)
                                if (ataque > 2)
                                {
                                    uri += "burbuja.png";
                                    burbuja = true;
                                    if (elementBuff)
                                    {
                                        if (jugador.Vida > atk1Enemigo + ELEMENT_BUFF)
                                        {
                                            jugador.Vida -= atk1Enemigo + ELEMENT_BUFF;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                    else if (elementNerf)
                                    {
                                        if (jugador.Vida > atk1Enemigo - ELEMENT_NERF)
                                        {
                                            jugador.Vida -= atk1Enemigo - ELEMENT_NERF;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                    else
                                    {
                                        if (jugador.Vida > atk1Enemigo)
                                        {
                                            jugador.Vida -= atk1Enemigo;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                }
                                // Ataque2
                                else
                                {
                                    uri += "hidropulso.png";
                                    hidropulso = true;
                                    if (elementBuff)
                                    {
                                        if (jugador.Vida > atk2Enemigo + ELEMENT_BUFF)
                                        {
                                            jugador.Vida -= atk2Enemigo + ELEMENT_BUFF;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                    else if (elementNerf)
                                    {
                                        if (jugador.Vida > atk2Enemigo - ELEMENT_NERF)
                                        {
                                            jugador.Vida -= atk2Enemigo - ELEMENT_NERF;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                    else
                                    {
                                        if (jugador.Vida > atk2Enemigo)
                                        {
                                            jugador.Vida -= atk2Enemigo;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                }
                                break;

                            case Personajes.DOTON:
                                if (jugador.Nombre.Equals("Suiton"))
                                    elementBuff = true;
                                else if (jugador.Nombre.Equals("Katon"))
                                    elementNerf = true;
                                // Ataque1
                                if (ataque > 2)
                                {
                                    uri += "hojaafilada.png";
                                    if (elementBuff)
                                    {
                                        if (jugador.Vida > atk1Enemigo + ELEMENT_BUFF)
                                        {
                                            jugador.Vida -= atk1Enemigo + ELEMENT_BUFF;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                    else if (elementNerf)
                                    {
                                        if (jugador.Vida > atk1Enemigo - ELEMENT_NERF)
                                        {
                                            jugador.Vida -= atk1Enemigo - ELEMENT_NERF;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                    else
                                    {
                                        if (jugador.Vida > atk1Enemigo)
                                        {
                                            jugador.Vida -= atk1Enemigo;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                }
                                // Ataque2
                                else
                                {
                                    uri += "rayosolar.png";
                                    if (elementBuff)
                                    {
                                        if (jugador.Vida > atk2Enemigo + ELEMENT_BUFF)
                                        {
                                            jugador.Vida -= atk2Enemigo + ELEMENT_BUFF;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                    else if (elementNerf)
                                    {
                                        if (jugador.Vida > atk2Enemigo - ELEMENT_NERF)
                                        {
                                            jugador.Vida -= atk2Enemigo - ELEMENT_NERF;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                    else
                                    {
                                        if (jugador.Vida > atk2Enemigo)
                                        {
                                            jugador.Vida -= atk2Enemigo;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                }
                                break;
                        }
                        break;

                    // Ataca enemigo2
                    case 1:
                        switch (tipoEnemigo2)
                        {
                            case Personajes.DOTON:
                                if (jugador.Nombre.Equals("Suiton"))
                                    elementBuff = true;
                                else if (jugador.Nombre.Equals("Katon"))
                                    elementNerf = true;
                                // Ataque1
                                if (ataque > 2)
                                {
                                    uri += "hojaafilada.png";
                                    if (elementBuff)
                                    {
                                        if (jugador.Vida > atk1Enemigo + ELEMENT_BUFF)
                                        {
                                            jugador.Vida -= atk1Enemigo + ELEMENT_BUFF;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                    else if (elementNerf)
                                    {
                                        if (jugador.Vida > atk1Enemigo - ELEMENT_NERF)
                                        {
                                            jugador.Vida -= atk1Enemigo - ELEMENT_NERF;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                    else
                                    {
                                        if (jugador.Vida > atk1Enemigo)
                                        {
                                            jugador.Vida -= atk1Enemigo;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                }
                                // Ataque2
                                else
                                {
                                    uri += "rayosolar.png";
                                    if (elementBuff)
                                    {
                                        if (jugador.Vida > atk2Enemigo + ELEMENT_BUFF)
                                        {
                                            jugador.Vida -= atk2Enemigo + ELEMENT_BUFF;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                    else if (elementNerf)
                                    {
                                        if (jugador.Vida > atk2Enemigo - ELEMENT_NERF)
                                        {
                                            jugador.Vida -= atk2Enemigo - ELEMENT_NERF;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                    else
                                    {
                                        if (jugador.Vida > atk2Enemigo)
                                        {
                                            jugador.Vida -= atk2Enemigo;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                }
                                break;

                            case Personajes.KATON:
                                if (jugador.Nombre.Equals("Doton"))
                                    elementBuff = true;
                                else if (jugador.Nombre.Equals("Suiton"))
                                    elementNerf = true;
                                // Ataque1
                                if (ataque > 2)
                                {
                                    uri += "ascua.png";
                                    if (elementBuff)
                                    {
                                        if (jugador.Vida > atk1Enemigo + ELEMENT_BUFF)
                                        {
                                            jugador.Vida -= atk1Enemigo + ELEMENT_BUFF;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                    else if (elementNerf)
                                    {
                                        if (jugador.Vida > atk1Enemigo - ELEMENT_NERF)
                                        {
                                            jugador.Vida -= atk1Enemigo - ELEMENT_NERF;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                    else
                                    {
                                        if (jugador.Vida > atk1Enemigo)
                                        {
                                            jugador.Vida -= atk1Enemigo;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                }
                                // Ataque2
                                else
                                {
                                    uri += "caos.png";
                                    if (elementBuff)
                                    {
                                        if (jugador.Vida > atk2Enemigo + ELEMENT_BUFF)
                                        {
                                            jugador.Vida -= atk2Enemigo + ELEMENT_BUFF;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                    else if (elementNerf)
                                    {
                                        if (jugador.Vida > atk2Enemigo - ELEMENT_NERF)
                                        {
                                            jugador.Vida -= atk2Enemigo - ELEMENT_NERF;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                    else
                                    {
                                        if (jugador.Vida > atk2Enemigo)
                                        {
                                            jugador.Vida -= atk2Enemigo;
                                        }
                                        else
                                        {
                                            listaPersonajes.Remove(jugador);
                                            miLienzo.Children.Remove(jugadorfig);
                                            miLienzo.Children.Remove(personajeText);
                                            jugadorEliminado = true;
                                        }
                                    }
                                }
                                break;
                        }
                        break;

                    default:
                        break;
                }

                enemAtkBrush.ImageSource = new BitmapImage(new Uri(uri, UriKind.Relative));
                enemAtkSprite.Fill = enemAtkBrush;

                // Algunos ajustes para los sprites
                if (burbuja)
                {
                    enemAtkSprite.Height = 60;
                    enemAtkSprite.Width = 60;
                    Canvas.SetLeft(enemAtkSprite, 225);
                    Canvas.SetTop(enemAtkSprite, 135);
                }
                else if (hidropulso)
                {
                    enemAtkSprite.Height = 85;
                    enemAtkSprite.Width = 85;
                    Canvas.SetLeft(enemAtkSprite, 220);
                    Canvas.SetTop(enemAtkSprite, 110);
                }
                else if (caos)
                {
                    enemAtkSprite.Height = 90;
                    enemAtkSprite.Width = 90;
                    Canvas.SetLeft(enemAtkSprite, 210);
                    Canvas.SetTop(enemAtkSprite, 110);
                }
                else
                {
                    enemAtkSprite.Height = 80;
                    enemAtkSprite.Width = 80;
                    Canvas.SetLeft(enemAtkSprite, 210);
                    Canvas.SetTop(enemAtkSprite, 120);
                }

                miLienzo.Children.Add(enemAtkSprite);

                // Efectos de sonido para los ataques
                if (uri.Contains("burbuja"))
                    uriSFX += "burbuja.mp3";
                else if (uri.Contains("hidropulso"))
                    uriSFX += "hidropulso.mp3";
                else if (uri.Contains("hojaafilada"))
                    uriSFX += "hojaafilada.mp3";
                else if (uri.Contains("rayosolar"))
                    uriSFX += "rayosolar.mp3";
                else if (uri.Contains("ascua"))
                    uriSFX += "ascua.mp3";
                else if (uri.Contains("caos"))
                    uriSFX += "caos.wav";

                ataqueSFX.Open(new Uri(uriSFX, UriKind.Relative));
                ataqueSFX.Play();
            }
            
            reactivarAtaques = true;

            return jugadorEliminado;
        }

        /*
         * Este método es ejecutado en cada intervalo del timer.
         * 
         * Dependiendo del valor del flag reactivarAtaques:
         *  true: significa que la secuencia de ataque (tanto del jugador como del enemigo)
         *        ha concluido, por lo que elimina los sprites y reactiva los botones de ataque
         *  false: solo ha concluido el ataque del jugador, por lo que elimina el sprite de éste
         *         y desencadena el ataque de los enemigos
         *         
         * Además determina el resultado de la ronda: 
         *  si el jugador es derrotado, muestra la pantalla de fin de partida 
         *  si el jugador gana, da paso a la siguiente oleada
         * 
         */
        public void SecuenciaAtaque(Canvas miLienzo, DispatcherTimer timer,
                                    Button atk1enem1, Button atk2enem1, Button atk1enem2, Button atk2enem2,
                                    bool eliminado1, bool eliminado2, Rectangle personaje,
                                    Button cambioMapaButton, Button cambioMusicaButton, Button pararMusicaButton,
                                    MediaPlayer musicaCombate, Button nuevaOleada, TextBlock nuevaOleadaText)
        {

            ImageBrush endBrush = new ImageBrush();
            MediaPlayer end = new MediaPlayer();

            ataqueSFX.Pause();

            if (reactivarAtaques)
            {
                miLienzo.Children.Remove(enemAtkSprite);

                // Los flags eliminado1 y eliminado2 toman su valor tras el ataque del jugador,
                // determinando si enemigo1 o enemigo2 han sido derrotados. A mayores,
                // si el jugador ha sido eliminado, no habilitará de nuevo los botones de ataque

                if ((!eliminado1 && !eliminado2) && !jugadorEliminado)
                {
                    atk1enem1.IsEnabled = true;
                    atk2enem1.IsEnabled = true;
                    atk1enem2.IsEnabled = true;
                    atk2enem2.IsEnabled = true;
                }
                else if ((eliminado1 && !eliminado2) && !jugadorEliminado)
                {
                    atk1enem2.IsEnabled = true;
                    atk2enem2.IsEnabled = true;
                }
                else if ((eliminado2 && !eliminado1) && !jugadorEliminado)
                {
                    atk1enem1.IsEnabled = true;
                    atk2enem1.IsEnabled = true;
                }

                if (usosAtk2 < 1)
                {
                    atk2enem1.IsEnabled = false;
                    atk2enem2.IsEnabled = false;
                }
            }
            else
                miLienzo.Children.Remove(ataqueSprite);

            // Esta parada es esencial para mantener el sprite enemigo en pantalla
            timer.Stop();

            // Ataque al jugador tras su ataque
            if (!reactivarAtaques)
                jugadorEliminado = AtacarAlJugador(miLienzo, personaje, timer);

            if (jugadorEliminado)
            {
                // FIN DE LA PARTIDA
                musicaCombate.Pause();
                ataqueSFX.Pause();
                end.Open(new Uri("Sonidos/youdied.mp3", UriKind.Relative));
                end.Play();

                cambioMapaButton.Visibility = Visibility.Hidden;
                cambioMusicaButton.Visibility = Visibility.Hidden;
                pararMusicaButton.Visibility = Visibility.Hidden;

                miLienzo.Background = Brushes.Black;
                endBrush.ImageSource = new BitmapImage(new Uri("Imagenes/fin.jpg", UriKind.Relative));

                Label youDied = new Label
                {
                    Background = endBrush,
                    Width = 700,
                    Height = 350
                };

                Canvas.SetLeft(youDied, 75);
                Canvas.SetTop(youDied, 15);

                miLienzo.Children.Add(youDied);

                DoubleAnimation fadeAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = new Duration(TimeSpan.FromSeconds(2)),
                    AutoReverse = false
                };

                youDied.BeginAnimation(Label.OpacityProperty, fadeAnimation);
            }

            else if (eliminado1 && eliminado2)
            {
                nuevaOleadaText.Visibility = Visibility.Visible;
                nuevaOleada.Visibility = Visibility.Visible;
            }
        }

        // Prepara la ListView actualizando los atributos de los personajes
        public void RestablecerJugadorListView()
        {
            jugador.Vida = 100;
            usosAtk2 = 3;

            enem1alive = true;  
            enem2alive = true;

            numWave += 1;

            // Incremento de daño adicional al jugador cada 2 rondas
            if (numWave % 2 != 0)
            {
                atk2Jugador += 5;
            }
            atk1Jugador += 5;
            atk2Jugador += 5;
            jugador.PotenciaAtk1 = atk1Jugador;
            jugador.PotenciaAtk2 = atk2Jugador;

            // Incremento del daño de los enemigos por oleada
            atk1Enemigo = 5 * numWave;
            atk2Enemigo = 10 * numWave;
        }

        public void Magia(Personaje enemigoSeleccionado)
        {
            int indexEnem, indexJugador;

            indexEnem = listaPersonajes.IndexOf(enemigoSeleccionado);
            indexJugador = listaPersonajes.IndexOf(jugador);
            if (indexEnem != indexJugador)
            {
                enemigoSeleccionado.Vida = 0;
                listaPersonajes.Remove(enemigoSeleccionado);
            }
        }

        public ImageBrush Cambiar_Mapa()
        {
            ImageBrush nuevoEscenarioBrush = new ImageBrush();
            Escenarios tipo;
            Random rand = new Random();

            tipo = (Escenarios)rand.Next(7);

            string uri = "Imagenes/escenarios/";

            switch (tipo)
            {
                case Escenarios.CAVE:
                    uri += "cave.jpg";
                    break;
                case Escenarios.DESERT:
                    uri += "desert.jpg";
                    break;
                case Escenarios.FOREST:
                    uri += "forest.jpg";
                    break;
                case Escenarios.HELL:
                    uri += "hell.jpg";
                    break;
                case Escenarios.JAPAN:
                    uri += "japan.jpg";
                    break;
                case Escenarios.MOUNTAIN:
                    uri += "mountain.jpg";
                    break;
                case Escenarios.MYSTIC:
                    uri += "mystic.jpg";
                    break;
                default:
                    break;
            }

            nuevoEscenarioBrush.ImageSource = new BitmapImage(new Uri(uri, UriKind.Relative));
            return nuevoEscenarioBrush;
        }


        public void SiguienteCancion(MediaPlayer musicaCombate)
        {
            Random rand = new Random();
            int nextSong;

            nextSong = rand.Next(0,3);

            string uri = "Sonidos/";

            switch (nextSong)
            {
                case 0:
                    uri += "15.mp3";
                    break;
                case 1:
                    uri += "16.mp3";
                    break;
                case 2:
                    uri += "21.mp3";
                    break;
                default:
                    break;
            }

            musicaCombate.Open(new Uri(uri, UriKind.Relative));
            musicaCombate.Play();
        }
    }
}
