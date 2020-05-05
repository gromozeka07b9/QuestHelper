import React from 'react';
import { List, Edit, Create, SimpleForm, Datagrid, TextField, TextInput, ReferenceField, Filter, SelectInput,  EditButton } from 'react-admin';
import { BooleanInput } from 'react-admin';
import { DateField, BooleanField } from 'react-admin';
import ImgField from './ImgField';
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

const RouteFilter = (props) => (
    <Filter {...props}>
        <TextInput label="По названию..." source="name" alwaysOn />
        <TextInput label="По тексту..." source="description" alwaysOn />
        <QuickFilter source="isPublished" label="Опубликованные" defaultValue={true} />
        <QuickFilter source="isDeleted" label="Удаленные" defaultValue={true} />
    </Filter>
);

export const PoiList = props => (
<List title="Точки интереса" filters={<RouteFilter />} {...props}>
        <Datagrid rowClick="edit">
            <TextField source="name" label="Название" />
            <DateField source="createDate" label="Дата создания" />
            <BooleanField source="isDeleted" label="Удален" />
            <BooleanField source="isPublished" label="Опубликована" />             
            <TextField source="description" label="Текст описания" multiline />
        </Datagrid>
    </List>
);

export const PoiEdit = props => (
    <Edit {...props}>
        <SimpleForm>
            <TextField source="id" label="id" fullWidth />
            <TextInput source="name" fullWidth />
            <TextInput source="createDate" fullWidth />
            <TextInput source="creatorId" fullWidth />
            <BooleanInput  source="isDeleted" fullWidth />
            <BooleanInput  source="isPublished" fullWidth />
            <TextInput source="imgFilename" fullWidth />
            <TextInput source="ImgBase64" fullWidth />
            <TextInput source="description" multiline fullWidth />
        </SimpleForm>
    </Edit>
);
