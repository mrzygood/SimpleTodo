import Keycloak from "keycloak-js";

const keycloakInstance = new Keycloak();

interface CallbackOneParam<T1 = void, T2 = void> {
    (param1: T1): T2;
}
/**
 * Initializes Keycloak instance and calls the provided callback function if successfully authenticated.
 *
 * @param onAuthenticatedCallback
 */
const Login = async () => {
    try {
        const result = await keycloakInstance.login({ redirectUri: "http://localhost:8080" });
        console.log(result);
    } catch (e) {
        console.dir(e);
        console.log(`keycloak login exception: ${e}`);
    }
};

const Logout = async () => {
    try {
        const result = await keycloakInstance.logout({ redirectUri: "http://localhost:8080" });
        console.log(result);
    } catch (e) {
        console.dir(e);
        console.log(`keycloak logout exception: ${e}`);
    };
};

const Load = async () => {
    try {
        const isAuthenticated = await keycloakInstance.init({
            onLoad: "check-sso",
            silentCheckSsoRedirectUri: `${location.origin}/silent-check-sso.html`
        });
        if (isAuthenticated) {
            console.log("authenicated")
        } else {
            console.log("non authenticated")
        }
    } catch (e) {
        console.dir(e);
        console.log(`keycloak init exception: ${e}`);
    }
};

const FetchProfile = async () => {
    try {
        const profile = await keycloakInstance.loadUserProfile();
        console.log('Retrieved user profile:', profile);
        console.log('token:', keycloakInstance.token);
    } catch (error) {
        console.error('Failed to load user profile:', error);
    }
};

const FetchBearerToken = async () => {
    try {
        return Promise.resolve(keycloakInstance.token);
    } catch (error) {
        console.error('Failed to fetch bearer token:', error);
    }
};

const KeyCloakService = {
    CallLogin: Login,
    CallLogout: Logout,
    Init: Load,
    GetUser: FetchProfile,
    GetBearerToken: FetchBearerToken
};

export default KeyCloakService;
