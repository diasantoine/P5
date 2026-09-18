namespace P5.Security
{
    /// <summary>
    /// Noms des rôles du site. L'inscription est ouverte à tous, mais s'inscrire ne donne aucun
    /// rôle : seul le compte du gérant, créé au démarrage, reçoit Admin, et seul Admin peut écrire.
    /// </summary>
    public static class AppRoles
    {
        public const string Admin = "Admin";
    }
}
