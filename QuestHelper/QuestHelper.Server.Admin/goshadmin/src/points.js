import React from 'react';
import { List, Edit, Create, SimpleForm, Datagrid, TextField, TextInput, DateInput, ReferenceInput, SelectInput,  Filter } from 'react-admin';
import { BooleanInput } from 'react-admin';
import { DateField, BooleanField } from 'react-admin';
//import ImgField from './ImgField';
//import { ImageField } from 'react-admin';
import { makeStyles, Chip } from '@material-ui/core';

const useQuickFilterStyles = makeStyles(theme => ({
    chip: {
        marginBottom: theme.spacing(1),
    },
}));
const QuickFilter = ({ label }) => {
    //const translate = useTranslate();
    const classes = useQuickFilterStyles();
    return <Chip className={classes.chip} label={label} />;
};

const RoutePointFilter = (props) => (
    <Filter {...props}>
        <TextInput label="По названию..." source="name" alwaysOn />
        <TextInput label="По тексту..." source="description" alwaysOn />
        <TextInput label="По id маршрута..." source="routeId" alwaysOn />
        <DateInput label="По дате создания..." source="createDate" alwaysOn />
        <ReferenceInput label="Route" source="routeId" reference="routes">
            <SelectInput optionText="name" />
        </ReferenceInput>    
        <QuickFilter source="isDeleted" label="Удаленные" defaultValue={true} />
    </Filter>
);

export const RoutePointList = props => (
    <List title="Точки" filters={<RoutePointFilter />} {...props} filter={{ is_published: true }} >
        <Datagrid rowClick="edit">
            <TextField source="id" label="id" />
            <TextField source="routeId" label="route Id" />
            <TextField source="name" label="Название" />
            <DateField source="createDate" label="Дата создания" />
            <TextField source="latitude" label="Широта" />
            <TextField source="longitude" label="Долгота" />
            <TextField source="address" label="Адрес" />
            <TextField source="version" label="Версия" />
            <BooleanField  source="isDeleted" label="Пометка удаления" />
        </Datagrid>
    </List>
);

export const RoutePointEdit = props => (
    <Edit {...props}>
        <SimpleForm>
            <TextInput disabled source="id" />
            <ReferenceInput source="routeId" reference="routes">
               <SelectInput optionText="name" />
            </ReferenceInput>
            <TextInput source="name" fullWidth/>
            <DateInput source="createDate" fullWidth/>
            <TextInput source="version" fullWidth/>
            <TextInput source="updatedUserId" fullWidth/>
            <DateInput source="updateDate" fullWidth/>

            <TextInput source="address" fullWidth/>
            <TextInput source="latitude" fullWidth/>
            <TextInput source="longitude" fullWidth/>
            <BooleanInput source="isDeleted" fullWidth/>
            <TextInput source="description" multiline fullWidth/>
        </SimpleForm>
    </Edit>
);

export const RoutePointCreate = props => (
    <Create {...props}>
        <SimpleForm>
            <ReferenceInput source="routeId" reference="routes">
               <SelectInput optionText="name" />
            </ReferenceInput>
            <TextInput source="name" fullWidth/>
            <DateInput source="createDate" fullWidth/>
            <TextInput source="version" fullWidth/>
            <TextInput source="updatedUserId" fullWidth/>
            <DateInput source="updateDate" fullWidth/>

            <TextInput source="address" fullWidth/>
            <TextInput source="latitude" fullWidth/>
            <TextInput source="longitude" fullWidth/>
            <BooleanInput source="isDeleted" fullWidth/>
            <TextInput source="description" multiline fullWidth/>
        </SimpleForm>
    </Create>
);
