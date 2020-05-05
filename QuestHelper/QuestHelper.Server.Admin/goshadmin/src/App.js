// in src/App.js
import React from 'react';
import Dashboard from './dashboard';

import authProvider from './authProvider';
import { Admin, Resource } from 'react-admin';
import { RouteList, RouteEdit } from './routes';
import { RoutePointList, RoutePointEdit, RoutePointCreate } from './points';
import { PoiList, PoiEdit } from './pois';
import goshDataProvider from './goshDataProvider';

const App = () => (
    <Admin title='test' dashboard={Dashboard} dataProvider={goshDataProvider} authProvider={authProvider}>
        <Resource name="routes" edit={RouteEdit} list={RouteList} />
        <Resource name="routepoints" create={RoutePointCreate} edit={RoutePointEdit} list={RoutePointList} />
        <Resource name="pois" edit={PoiEdit} list={PoiList} />
    </Admin>
);

export default App;
