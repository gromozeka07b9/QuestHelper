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
        <QuickFilter source="isShared" label="С общим доступом" defaultValue={true} />
    </Filter>
);

export const RouteList = props => (
<List title="Маршруты" filters={<RouteFilter />} {...props}>
        <Datagrid rowClick="edit">
            <TextField source="id" label="id" />
            <TextField source="name" label="Название" />
            <DateField source="createDate" label="Дата создания" />
            <TextField source="creator" label="Автор id" />
            <BooleanField source="isDeleted" label="Удален" />
            <BooleanField source="isShared" label="Общий доступ" />
            <BooleanField source="isPublished" label="Опубликован" />             
            <TextField source="description" label="Текст описания" multiline />
            <TextField source="CoverImgBase64" label="Обложка" />
        </Datagrid>
    </List>
);

export const RouteEdit = props => (
    <Edit {...props}>
        <SimpleForm>
            <TextField source="id" label="id" />
            <TextInput source="name" />
            <TextInput source="createDate" />
            <TextInput source="creatorId" />
            <TextInput source="version" />
            <TextInput source="imgFilename" />

            <BooleanInput  source="isDeleted" />
            <BooleanInput  source="isShared" />
            <BooleanInput  source="isPublished" />


            <TextInput source="description" multiline />
            <TextInput source="coverImgBase64" />
        </SimpleForm>
    </Edit>
);
