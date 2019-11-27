using ClassesMetier;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace GestionnaireBDD
{
    public class GstBdd
    {
        MySqlConnection cnx;
        MySqlCommand cmd;
        MySqlDataReader dr;

        public GstBdd()
        {
            string driver = "server=localhost;user id=root;password=;database=reservationsalle";
            cnx = new MySqlConnection(driver);
            cnx.Open();
        }

        public List<Manifestation> GetAllManifestations()
        {
            List<Manifestation> lstManif = new List<Manifestation>();
            cmd = new MySqlCommand("select * from manifestation inner join salle on numSalle = idSalle", cnx);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Salle s = new Salle()
                {
                    IdSalle = Convert.ToInt32(dr[4]),
                    NomSalle = dr[6].ToString(),
                    NbPlaces = Convert.ToInt32(dr[7])
                };

                Manifestation m = new Manifestation()
                {
                    IdManif = Convert.ToInt32(dr[0]),
                    NomManif = dr[1].ToString(),
                    DateDebutManif = dr[2].ToString(),
                    DateFinManif = dr[3].ToString(),
                    LaSalle = s
                };
                lstManif.Add(m);
            }
            dr.Close();
            return lstManif;
        }

        public List<Place> GetAllPlacesByIdManifestation(int idManif,int idSalle)
        {
            List<Place> lstPlace = new List<Place>();
            cmd = new MySqlCommand("select numPlace, numTarif, libre, prix from manifestation inner join occuper on idManif = numManif inner join place on numPlace = place.idPlace inner join tarif on place.numTarif = tarif.idTarif where idManif = " + idManif + " and place.numSalle = " + idSalle + ";", cnx);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                bool a;
                char etat;
                if(Convert.ToInt32(dr[2]) == 1)
                {
                    a = true;
                    etat = 'o';
                }
                else
                {
                    a = false;
                    etat = 'l';
                }

                Place p = new Place()
                {
                    IdPlace = Convert.ToInt32(dr[0]),
                    CodeTarif = Convert.ToChar(dr[1]),
                    Occupee = a,
                    Prix = Convert.ToDouble(dr[3]),
                    Etat = etat
                };
                lstPlace.Add(p);
            }
            dr.Close();
            return lstPlace;
        }

        public List<Tarif> GetAllTarifs()
        {
            List<Tarif> lstTarif = new List<Tarif>();
            cmd = new MySqlCommand("select * from tarif", cnx);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Tarif t = new Tarif()
                {
                    IdTarif = Convert.ToChar(dr[0]),
                    NomTarif = dr[1].ToString(),
                    Prix = Convert.ToDouble(dr[2])
                };
                lstTarif.Add(t);
            }
            dr.Close();
            return lstTarif;
        }

        public void ReserverPlace(int idPlace, int idSalle,int idManif)
        {
            cmd = new MySqlCommand("update occuper SET libre = 1 where numManif = " + idManif + " and numPlace = " + idPlace + " and numSalle = " + idSalle + ";", cnx);
            cmd.ExecuteNonQuery();
        }
    }
}
