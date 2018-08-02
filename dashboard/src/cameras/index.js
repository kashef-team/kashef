import React from 'react';
import { 
    List,
    Datagrid,
    TextField,
    Create,
    SimpleForm,
    SimpleFormIterator,
    TextInput,
    ArrayInput,
    SelectField,
    BooleanInput,
    Edit,
    EditButton,
    DisabledInput,
 } from 'react-admin';

export const CameraList = (props) => (
    <List {...props}>
        <Datagrid>
            <TextField source="id" />
            <TextField source="label" />
            <TextField source="longitude" />
            <TextField source="latitude" />
            <EditButton />
        </Datagrid>
    </List>
);

export const CameraCreate = (props) => (
    <Create {...props}>
        <SimpleForm>
            <TextInput source="label" />
            <TextInput source="longitude" />
            <TextInput source="latitude" />
        </SimpleForm>
    </Create>
);

const CameraTitle = ({ record }) => {
    return <span>Camera id: {record ? `"${record.id}"` : ''}</span>;
};

export const CameraEdit = (props) => (
    <Edit title={<CameraTitle />} {...props}>
        <SimpleForm>
            <TextInput source="label" />
            <TextInput source="longitude" />
            <TextInput source="latitude" />
        </SimpleForm>
    </Edit>
);