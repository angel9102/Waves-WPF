using System.ComponentModel;

namespace Waves
{
    public enum Personajes { SUITON, DOTON, KATON };

    public class Personaje : INotifyPropertyChanged     
    {
        public event PropertyChangedEventHandler PropertyChanged;

        const int VIDA = 100;

        // Atributos de los personajes
        private string tipoPersonaje_;
        private string nombre_;
        private int vida_;
        private string ataque1_;
        private string ataque2_;
        private int potenciaAtk1_;
        private int potenciaAtk2_;

        // Recubrimientos de los atributos anteriores para recoger los valores
        // Cambia el valor de la propiedad y lo comunica
        public string TipoPersonaje
        {
            get { return tipoPersonaje_; }
            set { tipoPersonaje_ = value; OnPropertyChanged("tipoPersonaje"); }   
        }
        public string Nombre
        {
            get { return nombre_; }
            set { nombre_ = value; OnPropertyChanged("nombre"); }
        }

        public int Vida
        {
            get { return vida_; }
            set { vida_ = value; OnPropertyChanged("vida"); }
        }
        public string Ataque1
        {
            get { return ataque1_; }
            set { ataque1_ = value; OnPropertyChanged("ataque1"); }
        }
        public string Ataque2
        {
            get { return ataque2_; }
            set { ataque2_ = value; OnPropertyChanged("ataque2"); }
        }
        public int PotenciaAtk1
        {
            get { return potenciaAtk1_; }
            set { potenciaAtk1_ = value; OnPropertyChanged("potenciaAtk1"); }
        }
        public int PotenciaAtk2
        {
            get { return potenciaAtk2_; }
            set { potenciaAtk2_ = value; OnPropertyChanged("potenciaAtk2"); }
        }

        /* --- CONSTRUCTOR DE LA CLASE --- */
        public Personaje(int tipoPJ, int personaje)
        {
            switch (tipoPJ)
            {
                case 0:
                    TipoPersonaje = "Jugador";
                    break;
                case 1:
                    TipoPersonaje = "Enemigo 1";
                    break;
                case 2:
                    TipoPersonaje = "Enemigo 2";
                    break;
                default:
                    break;

            }

            if (personaje == (int)Personajes.SUITON)
            {
                Nombre = "Suiton";
                Vida = VIDA;
                Ataque1 = "Burbuja";
                Ataque2 = "Hidropulso";
                PotenciaAtk1 = 25;
                PotenciaAtk2 = 35;
            }

            if (personaje == (int)Personajes.DOTON)
            {
                Nombre = "Doton";
                Vida = VIDA;
                Ataque1 = "Hoja Afilada";
                Ataque2 = "Rayo Solar";
                PotenciaAtk1 = 25;
                PotenciaAtk2 = 35;
            }

            if (personaje == (int)Personajes.KATON)
            {
                Nombre = "Katon";
                Vida = VIDA;
                Ataque1 = "Ascua";
                Ataque2 = "Caos";
                PotenciaAtk1 = 25;
                PotenciaAtk2 = 35;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    
    
}
