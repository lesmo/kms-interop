using System;
using System.Collections.Generic;
using System.Text;

namespace Kms.Interop.OAuth.SocialClients {
    public enum FacebookPermission {
        /// <summary>
        ///     Acceso a información Básica del Usuario, como: id, nombre, username,
        ///     género o idioma.
        /// </summary>
        BasicInfo,

        /// <summary>
        ///     Acceso a la dirección de correo electrónico primario del Usuario.
        /// </summary>
        Email,

        /// <summary>
        ///     Acceso a la sección de Acerca de Mi del Usuario.
        /// </summary>
        UserAboutMe,

        /// <summary>
        ///     Acceso a la sección de Actividades del Usuario.
        /// </summary>
        UserActivities,

        /// <summary>
        ///     Acceso a la fecha de nacimiento del Usuario.
        /// </summary>
        UserBirthday,

        /// <summary>
        ///     Acceso a los Check-ins del Usuario.
        /// </summary>
        UserCheckins,

        /// <summary>
        ///     Acceso al Historial Académico del Usuario.
        /// </summary>
        UserEducationHistory,

        /// <summary>
        ///     Acceso a los Eventos a los que el Usuario a confirmado asistencia.
        /// </summary>
        UserEvents,

        /// <summary>
        ///     Acceso a los Grupos a los que pertenece el Usuario.
        /// </summary>
        UserGroups,

        /// <summary>
        ///     Acceso al lugar donde creció el Usuario.
        /// </summary>
        UserHometown,

        /// <summary>
        ///     Acceso a los Intereses del Usuario.
        /// </summary>
        UserInterests,

        /// <summary>
        ///     Acceso a los Likes del Usuario.
        /// </summary>
        UserLikes,

        /// <summary>
        ///     Acceso a la ubicación actual configurada por el Usuario.
        /// </summary>
        UserLocation,

        /// <summary>
        ///     Acceso a las Notas del Usuario.
        /// </summary>
        UserNotes,

        /// <summary>
        ///     Acceso a las Fotografías del Usuario.
        /// </summary>
        UserPhotos,

        /// <summary>
        ///     Acceso a las preguntas que el Usuario ha hecho.
        /// </summary>
        UserQuestions,

        /// <summary>
        ///     Acceso a las Relaciones Familiares y Personales del Usuario.
        /// </summary>
        UserRelationships,

        /// <summary>
        ///     Acceso a los Detalles de las Relaciones Familiares y Personales del Usuario.
        /// </summary>
        UserRelationshipDetails,

        /// <summary>
        ///     Acceso a las Afiliaciones Religiosas y Políticas del Usuario.
        /// </summary>
        UserReligionPolitics,

        /// <summary>
        ///     Acceso a las Mensajes de Estado y Check-ins del Usuario.
        /// </summary>
        UserStatus,

        /// <summary>
        ///     Acceso a los Usuarios a los que está Suscrito el Usuario, y los Usuarios
        ///     que están suscritos al Usuario actual.
        /// </summary>
        UserSubscriptions,

        /// <summary>
        ///     Acceso a los Vídeos subidos por el Usuario, y a los que ha sido etiquetado.
        /// </summary>
        UserVideos,

        /// <summary>
        ///     Acceso a la dirección del Sitio Web del Usuario.
        /// </summary>
        UserWebsite,

        /// <summary>
        ///     Acceso al Historial de Trabajo del Usuario.
        /// </summary>
        UserWorkHistory,

        /// <summary>
        ///     Acceso a la sección de Acerca de Mi de los Amigos.
        /// </summary>
        FriendsAboutMe,

        /// <summary>
        ///     Acceso a la sección de Actividades de los Amigos.
        /// </summary>
        FriendsActivities,

        /// <summary>
        ///     Acceso a la fecha de nacimiento de los Amigos.
        /// </summary>
        FriendsBirthday,

        /// <summary>
        ///     Acceso a los Check-ins de los Amigos.
        /// </summary>
        FriendsCheckins,

        /// <summary>
        ///     Acceso al Historial Académico de los Amigos.
        /// </summary>
        FriendsEducationHistory,

        /// <summary>
        ///     Acceso a los Eventos a los que los Amigos han confirmado asistencia.
        /// </summary>
        FriendsEvents,

        /// <summary>
        ///     Acceso a los Grupos a los que pertenecen los Amigos.
        /// </summary>
        FriendsGroups,

        /// <summary>
        ///     Acceso al lugar donde crecieron los Amigos.
        /// </summary>
        FriendsHometown,

        /// <summary>
        ///     Acceso a los Intereses de los Amigos.
        /// </summary>
        FriendsInterests,

        /// <summary>
        ///     Acceso a los Likes de los Amigos.
        /// </summary>
        FriendsLikes,

        /// <summary>
        ///     Acceso a la ubicación actual configurada por los Amigos.
        /// </summary>
        FriendsLocation,

        /// <summary>
        ///     Acceso a las Notas de los Amigos.
        /// </summary>
        FriendsNotes,

        /// <summary>
        ///     Acceso a las Fotografías de los Amigos.
        /// </summary>
        FriendsPhotos,

        /// <summary>
        ///     Acceso a las preguntas que los Amigos han hecho.
        /// </summary>
        FriendsQuestions,

        /// <summary>
        ///     Acceso a las Relaciones Familiares y Personales de los Amigos.
        /// </summary>
        FriendsRelationships,

        /// <summary>
        ///     Acceso a los Detalles de las Relaciones Familiares y Personales de los Amigos.
        /// </summary>
        FriendsRelationshipDetails,

        /// <summary>
        ///     Acceso a las Afiliaciones Religiosas y Políticas de los Amigos.
        /// </summary>
        FriendsReligionPolitics,

        /// <summary>
        ///     Acceso a las Mensajes de Estado y Check-ins de los Amigos.
        /// </summary>
        FriendsStatus,

        /// <summary>
        ///     Acceso a los Usuarios a los que están Suscritos los Amigos, y los Usuarios
        ///     que están suscritos a los Amigos.
        /// </summary>
        FriendsSubscriptions,

        /// <summary>
        ///     Acceso a los Vídeos subidos por los Amigos, y a los que han sido etiquetados.
        /// </summary>
        FriendsVideos,

        /// <summary>
        ///     Acceso a la dirección del Sitio Web de los Amigos.
        /// </summary>
        FriendsWebsite,

        /// <summary>
        ///     Acceso al Historial de Trabajo de los Amigos.
        /// </summary>
        FriendsWorkHistory,

        /// <summary>
        ///     Acceso a todas las Listas de Amigos creadas por el Usuario.
        /// </summary>
        ReadFriendlists,

        /// <summary>
        ///     Acceso a los Facebook Insights de las páginas, aplicaciones o dominios
        ///     administrados por el Usuario.
        /// </summary>
        ReadInsights,

        /// <summary>
        ///     Acceso de lectura a los mensajes privados del Usuario.
        /// </summary>
        ReadMailbox,

        /// <summary>
        ///     Acceso a las Solicitudes de Amistad del Usuario.
        /// </summary>
        ReadRequests,

        /// <summary>
        ///     Acceso a todos los Posts en la línea de tiempo de Novedades del Usuario,
        ///     permitiendo a la aplicación realizar búsquedas sobre ella.
        /// </summary>
        ReadStream,

        /// <summary>
        ///     Acceso a la presencia de Conectado/Desconectado del Usuario.
        /// </summary>
        UserOnlinePresence,

        /// <summary>
        ///     Acceso a la presencia de Contectado/Desconectado de los Amigos.
        /// </summary>
        FriendsOnlinePresence,

        /// <summary>
        ///     Acceso a la gestión de Anuncios.
        /// </summary>
        AdsManagement,

        /// <summary>
        ///     Acceso a la creación de Eventos.
        /// </summary>
        CreateEvent,

        /// <summary>
        ///     Acceso a la gestión de Listas de Amigos.
        /// </summary>
        ManageFriendlists,

        /// <summary>
        ///     Acceso a la gestión de Notificaciones del Usuario.
        /// </summary>
        ManageNotifications,

        /// <summary>
        ///     Acceso a las acciones de Publicación. Los diálogos de Post,
        ///     Envío y Solicitud están excentos del uso de éste permiso.
        /// </summary>
        PublishActions,

        /// <summary>
        ///     Acceso a la Publicación en Páginas. Para un Usuario de Facebook,
        ///     debe utilizarse PublishActions.
        /// </summary>
        PublishStream,

        /// <summary>
        ///     Acceso al envío de Respuesta de Asistencia (RSVP) a Eventos a los que
        ///     ha sido invitado el Usuario.
        /// </summary>
        RsvpEvent,

        /// <summary>
        ///     Acceso a las Acciones publicadas al Graph API por todas las aplicaciones
        ///     sobre la escucha de Música. del Usuario.
        /// </summary>
        UserActionsMusic,

        /// <summary>
        ///     Acceso a las Acciones publicadas al Graph API por todas las aplicaciones
        ///     sobre Noticias del Usuario.
        /// </summary>
        UserActionsNews,

        /// <summary>
        ///     Acceso a las Acciones publicadas al Graph API por todas las aplicaciones
        ///     alrededor de experiencias en Vídeo del Usuario.
        /// </summary>
        UserActionsVideo,

        /// <summary>
        ///     Acceso a la publicación y lectura de Logros alcanzados en Juegos por el Usuario.
        /// </summary>
        UserGamesActivity,

        /// <summary>
        ///     Acceso a las Acciones publicadas al Graph API por todas las aplicaciones
        ///     sobre la escucha de Música de los Amigos.
        /// </summary>
        FriendsActionsMusic,

        /// <summary>
        ///     Acceso a las Acciones publicadas al Graph API por todas las aplicaciones
        ///     sobre Noticias de los Amigos.
        /// </summary>
        FriendsActionsNews,

        /// <summary>
        ///     Acceso a las Acciones publicadas al Graph API por todas las aplicaciones
        ///     alrededor de experiencias en Vídeo de los Amigos.
        /// </summary>
        FriendsActionsVideo,

        /// <summary>
        ///     Acceso a la publicación y lectura de Logros alcanzados en Juegos por los
        ///     Amigos.
        /// </summary>
        FriendsGamesActivity
    }
}
