import type { RouteLocationNormalizedLoaded } from 'vue-router'

export type MenuItem = {
    label: string;
    path: string;
    subMenu: MenuItem[];
    target?: '_blank' | '_self' | '_parent' | '_top';
    fontSize?: string;
    active?: boolean;
    matchExact?: boolean;
    hide?: boolean;
}
export type NavMenuItems = {
    group: string,
    hide?: boolean,
    menuItems: MenuItem[]
}

export function resolveActiveRoute(currentRoute : RouteLocationNormalizedLoaded, menuItems: MenuItem[]) : boolean {
    for (const menuItem of menuItems) {
        if (menuItem.subMenu.length === 0) {
            const routeMatch = menuItem.matchExact
                ? currentRoute.path === menuItem.path
                : currentRoute.path.startsWith(menuItem.path)

            if (routeMatch) {
                menuItem.active = true
                return true
            }
        }
        else if (resolveActiveRoute(currentRoute, menuItem.subMenu)) {
            menuItem.active = true
            return true
        }
    }

    return false;
}
