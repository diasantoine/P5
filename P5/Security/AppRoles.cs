namespace P5.Security
{
    /// <summary>
    /// Noms des rôles du site. Seul le compte du gérant, créé au démarrage, reçoit Admin,
    /// et seul Admin peut écrire dans l'inventaire.
    /// </summary>
    public static class AppRoles
    {
        public const string Admin = "Admin";
    }
}
