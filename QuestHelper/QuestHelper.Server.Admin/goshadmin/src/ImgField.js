import React from 'react';

import { makeStyles } from '@material-ui/core/styles';
import LaunchIcon from '@material-ui/icons/Launch';

//var mainUrl = "http://localhost:31192/api";
const useStyles = makeStyles({
    link: {
        textDecoration: 'none',
    },
    icon: {
        width: '0.5em',
        paddingLeft: 2,
    },
});

const ImgField = ({ record = {}, source }) => {
    const classes = useStyles();
    //http://igosh.pro/images/icon.png
    //return (<img src={record[source]} />);
    return (<img src='http://igosh.pro/images/icon.png' />);
}

export default ImgField;