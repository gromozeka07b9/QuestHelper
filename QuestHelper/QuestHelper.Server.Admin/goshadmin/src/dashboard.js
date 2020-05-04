// in src/Dashboard.js
import React from 'react';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import CardHeader from '@material-ui/core/CardHeader';

var welcomeUser = "Здравствуйте, " + localStorage.getItem('username') + "!";
export default () => (
    <Card>
        <CardHeader title={welcomeUser} />
        <CardContent>Здесь находится админка для настроек Gosh.</CardContent>
    </Card>
);